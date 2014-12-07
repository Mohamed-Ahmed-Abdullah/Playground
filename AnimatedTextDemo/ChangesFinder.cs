using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimatedTextDemo
{
    public class ChangesFinder
    {
        private List<string> SubMatches(string wrong, string right)
        {
            var globalMatches = new List<StringObject>();

            for (var i = 2; i < wrong.Length + 1; i++)
            {
                //if (i % 2 == 0)
                { 
                    var matches = new List<string>();
                    var subStringToMatch = wrong.Substring(i - 2, 2);
                    for (var j = 1; j <= wrong.Length; j++)
                    {
                        if (right.IndexOf(subStringToMatch) != -1)
                        {
                            matches.Add(subStringToMatch);
                            try { subStringToMatch = wrong.Substring(i - 2, j + 2); }
                            catch (Exception) { break; }
                        }
                        else
                            break;
                    }
                    if (matches.Any())
                        globalMatches.Add(new StringObject { Text = matches[matches.Count - 1] });
                }
            }

            //filter globalMatches
            foreach (var match in globalMatches.ToList())
            {
                if (globalMatches.Where(w => w != match).Any(a => a.Text.Contains(match.Text)))
                    globalMatches.Remove(match);
            }
            var stringGlobalMatches = globalMatches.Select(s => s.Text).ToList();

            //if there is no 2char matche lets find 1char match
            if (stringGlobalMatches.Count == 0)
            {
                //if there is any match chars in the same index this is a match 
                var minLength = (wrong.Length < right.Length) ? wrong.Length : right.Length;
                for (var i = 0; i < minLength; i++)
                    if (wrong[i] == right[i])
                        stringGlobalMatches.Add(wrong[i] + "");
            }

            return stringGlobalMatches;
        }

        private List<Change> GetInsertions(string right, List<string> subMatches)
        {
            var indexesMathched = new List<int>();
            //detect
            foreach (var sub in subMatches)
            {
                var index = right.IndexOf(sub);

                for (var i = index; i < index + sub.Count(); i++)
                    indexesMathched.Add(i);
            }
            //calculate 
            var changes = new List<Change>();
            for (var i = 0; i < right.Count(); i++)
            {
                if (indexesMathched.All(a => a != i))
                    changes.Add(new Change
                    {
                        ChangeType = ChangeType.Insert,
                        Index = i,
                        Character = right[i]
                    });
            }
            return changes;
        }

        private List<Change> GetRemoves(string wrong, List<string> subMatches)
        {
            var indexesMathched = new List<int>();
            //detect
            foreach (var sub in subMatches)
            {
                var index = wrong.IndexOf(sub);

                for (var i = index; i < index + sub.Count(); i++)
                    indexesMathched.Add(i);
            }
            //calculate 
            var changes = new List<Change>();
            for (var i = 0; i < wrong.Count(); i++)
            {
                //if(i not in indexesMathched)
                if (indexesMathched.All(a => a != i))
                    changes.Add(new Change
                    {
                        ChangeType = ChangeType.Remove,
                        Index = i,
                        Character = wrong[i]
                    });
            }
            return changes;
        }

        private List<Change> GetSwaps(string wrong, string right, List<string> subMatches)
        {
            var changes = new List<Change>();
            //any replacement require the two string to be the same lenght, for now at least 
            //after eleminating the subMatches we should figure out wich chars if we swap them we have a match
            //we will asume first that we have one switch to get the match but if we didn't we 

            //replacement hard to be understand and more than two replacement will not be understod at all
            if (wrong.Length != right.Length)
            {
                return changes;
            }

            for (var i = 1; i < wrong.Length; i++)
            {
                if (wrong.Swap(i - 1, i) == right)
                {
                    changes.Add(new Change
                    {
                        ChangeType = ChangeType.Swap,
                        Character = wrong[i - 1],
                        Character2 = wrong[i],
                        Index = i - 1,
                        Index2 = i
                    });
                    return changes;
                }
            }
            return changes;
        }

        public List<Change> Find(string wrong, string right)
        {
            var submatches = SubMatches(wrong, right);

            var insert = GetInsertions(right, submatches);
            var remove = GetRemoves(wrong, submatches);
            var swaps = GetSwaps(wrong, right, submatches);

            //now we should check the common logic, if it's replace no need for insert and them remove 
            foreach (var change in swaps)
            {
                var insertToRemove1 = insert.FirstOrDefault(f => f.Character == change.Character && f.Index == change.Index2);
                var insertToRemove2 = insert.FirstOrDefault(f => f.Character == change.Character2 && f.Index == change.Index);
                if (insertToRemove1 != null)
                    insert.Remove(insertToRemove1);
                if (insertToRemove2 != null)
                    insert.Remove(insertToRemove2);

                var removeToRemove1 = remove.FirstOrDefault(f => f.Character == change.Character && f.Index == change.Index);
                var removeToRemove2 = remove.FirstOrDefault(f => f.Character == change.Character2 && f.Index == change.Index2);
                if (removeToRemove1 != null)
                    remove.Remove(removeToRemove1);
                if (removeToRemove2 != null)
                    remove.Remove(removeToRemove2);
            }

            //any "insert" then "remove" in the same index it should changes to be "replace" 
            var replace = new List<Change>();
            foreach (var insertChange in insert.ToList())
            {
                var removeChange = remove.FirstOrDefault(f => f.Index == insertChange.Index);
                if (removeChange != null)
                {
                    remove.Remove(removeChange);
                    insert.Remove(insertChange);
                    replace.Add(new Change
                    {
                        ChangeType = ChangeType.Replace,
                        Character = removeChange.Character,
                        Character2 = insertChange.Character,
                        Index = insertChange.Index
                    });
                }
            }


            return insert.Union(remove).Union(swaps).Union(replace).ToList();
        }

        public class Change
        {
            public ChangeType ChangeType { get; set; }
            public int Index { get; set; }
            /// <summary>
            /// used when the change type is swap
            /// </summary>
            public int Index2 { get; set; }
            public char Character { get; set; }
            /// <summary>
            /// used when the change type is swap
            /// </summary>
            public char Character2 { get; set; }

            public override string ToString()
            {
                return ChangeType + " " + Character + " " + Index;
            }
        }

        public class Match
        {

        }

        public enum ChangeType
        {
            Insert = 1,
            Remove = 2,
            Swap = 3,
            Replace = 4, //when you have insert and remove in the same place (same index)
        }

        private class StringObject
        {
            public string Text { get; set; }
        }
    }

    public static class Extentions
    {
        public static string Swap(this string text, int index1, int index2)
        {
            var textArray = text.ToCharArray();
            var temp = textArray[index1];
            textArray[index1] = textArray[index2];
            textArray[index2] = temp;
            return new string(textArray);
        }
    }
}
