//==================================================================================================
// System  : DenKa
// File    : TestResult.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// No, File Name, File Value, Date Time, Action
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

using System;

namespace PlastMB.Model
{

    public class TestResult
    {
        [System.ComponentModel.DisplayName("No")]
        public string TestName
        {
            get; set;
        }

        [System.ComponentModel.DisplayName("File Name")]
        public string FileName
        {
            get; set;
        }

        [System.ComponentModel.DisplayName("File Value")]
        public string TestValue
        {
            get; set;
        }

        [System.ComponentModel.DisplayName("Date Time")]
        public DateTime TestDate
        {
            get; set;
        }

        //public string ShortDescription => $"Order ID: {TestName}";

        public override string ToString() => $"{TestName} {TestValue}";
    }
}