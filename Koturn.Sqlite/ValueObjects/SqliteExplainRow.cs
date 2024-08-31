namespace Koturn.Sqlite.ValueObjects
{
    /// <summary>
    /// Result row of "EXPLAIN".
    /// </summary>
    /// <remarks>
    /// <see href="https://www.sqlite.org/opcode.html"/>
    /// </remarks>
    public class SqliteExplainRow
    {
        /// <summary>
        /// Address oh instruction.
        /// </summary>
        public int Address { get; private set; }
        /// <summary>
        /// Name of opcode.
        /// </summary>
        public string OpCode { get; private set; }
        /// <summary>
        /// First operand, which is usually the cursor number.
        /// </summary>
        public int P1 { get; private set; }
        /// <summary>
        /// Second operand, which is usually the jump destination.
        /// </summary>
        public int P2 { get; private set; }
        /// <summary>
        /// Third operand.
        /// </summary>
        public int P3 { get; private set; }
        /// <summary>
        /// Fourth operand, which is various things.
        /// </summary>
        public string P4 { get; private set; }
        /// <summary>
        /// Fifth operand, normally used to hold flags, which can affect the opcode in subtle ways.
        /// </summary>
        public ushort P5 { get; private set; }
        /// <summary>
        /// Comment.
        /// </summary>
        public string Comment { get; private set; }

        /// <summary>
        /// Initialize all members.
        /// </summary>
        /// <param name="address">Address oh instruction.</param>
        /// <param name="opCode">Name of opcode.</param>
        /// <param name="p1">First operand, which is usually the cursor number.</param>
        /// <param name="p2">Second operand, which is usually the jump destination.</param>
        /// <param name="p3">Third operand.</param>
        /// <param name="p4">Fourth operand, which is various things.</param>
        /// <param name="p5">Fifth operand, normally used to hold flags, which can affect the opcode in subtle ways.</param>
        /// <param name="comment">Comment.</param>
        public SqliteExplainRow(int address, string opCode, int p1, int p2, int p3, string p4, ushort p5, string comment)
        {
            Address = address;
            OpCode = opCode;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
            P5 = p5;
            Comment = comment;
        }

        /// <summary>
        /// Get string which represents contents of this instance.
        /// </summary>
        /// <returns>String which represents contents of this instance.</returns>
        public override string ToString()
        {
            var str = string.Format("{0}: {1} [{2}] [{3}] [{4}] [{5}] [{6}]", Address, OpCode, P1, P2, P3, P4, P5);
            if (Comment != null)
            {
                str += "; " + Comment;
            }
            return str;
        }
    }
}
