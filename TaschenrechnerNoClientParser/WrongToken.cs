using System;
using System.Collections.Generic;
using System.Text;

namespace TaschenrechnerCS
{
    [Serializable]
    internal class WrongTokenException(string message) : Exception(message)
    {
    }

    [Serializable]
    internal class MissingClosingBracketException(string message) : Exception(message)
    {
    }
}
