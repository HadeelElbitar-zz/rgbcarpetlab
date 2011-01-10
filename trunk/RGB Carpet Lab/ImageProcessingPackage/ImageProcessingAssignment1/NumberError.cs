using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessingAssignment1
{
    class NumberError  : ApplicationException
    {
          public NumberError() : base() { }
            public NumberError(string s) : base(s) { }
            public NumberError(string s, Exception ex)
                : base(s, ex) { }
    }
}
