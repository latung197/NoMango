//==================================================================================================
// System  : DenKa
// File    : Data.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// Force, Value, Update
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using System;

namespace PlastMB.Model
{

    public class Data
    {
        [System.ComponentModel.DisplayName(" ")]
        public string Force
        {
            get; set;
        }

        [System.ComponentModel.DisplayName("Giá trị hiện tại")]
        public string Value
        {
            get; set;
        }

        [System.ComponentModel.DisplayName("Giá trị sau cập nhật")]
        public string Update
        {
            get; set;
        }
    }
}