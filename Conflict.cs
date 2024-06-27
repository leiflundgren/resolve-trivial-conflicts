using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resolve_trivial_conflicts
{
    internal class Conflict
    {        
        public StringWriter head = new StringWriter(), change = new StringWriter();
        public string? HeadLine, MiddleLine, EndLine;
        public int? HeadLineNo, MiddleLineNo, EndLineNo;



        public Conflict()
        {
        }


        public override string ToString()
        {
            return $"Conflict {HeadLineNo}-{EndLineNo}\n{Head}\n---------\n{Change}";
        }

        public string Head => head.ToString();
        public string Change => change.ToString();

        public bool EqualsIgnoreWhitespace()
        {
            var head = this.head.ToString().GetEnumerator();
            var change = this.change.ToString().GetEnumerator();

            if (!head.MoveNext()) return !change.MoveNext();
            if (!change.MoveNext()) return !head.MoveNext();


            for (; ; )
            {
                if (Char.IsWhiteSpace(change.Current))
                {
                    if (change.MoveNext()) continue; else break;
                }
                if (Char.IsWhiteSpace(head.Current))
                {
                    if (head.MoveNext()) continue; else break;
                }
                if (change.Current == head.Current)
                {
                    bool cm = change.MoveNext(), ch = head.MoveNext();
                    if (!cm || !ch)
                        return false;
                    continue;
                }

                return false;
            }

            if (!head.MoveNext())
            {
                while (change.MoveNext())
                {
                    if (!Char.IsWhiteSpace(change.Current))
                        return false;
                }
                return true;
            }
            else if (!change.MoveNext())
            {
                while (head.MoveNext())
                {
                    if (!Char.IsWhiteSpace(head.Current))
                        return false;
                }
                return true;
            }
            else
            {
                throw new InvalidOperationException($"Should only exit for if we reached end!");
            }
        }

        internal void WriteTo(TextWriter writer)
        {
            writer.WriteLine(HeadLine);
            writer.Write(head);
            writer.WriteLine(MiddleLine);
            writer.Write(change);
            writer.WriteLine(EndLine);
        }
    }
}
