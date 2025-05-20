//==================================================================================================
// System  : DenKa
// File    : State.cs
// Author  : Tan Vu (tanvd@robotcom-fa.com)
// Updated : 05/06/2024
// Note    : Copyright 2018-2024, RFVN, All rights reserved
//
// Index, CSV file, InputState, Excel file, ExportState
//
// Modification Log:
// Version  Date        By       Notes
// -------  ----------  -------  --------
// 1.0.0    05/06/2024  Tan Vu   Created the code
//==================================================================================================

namespace PlastMB.Model
{

    public class State
    {
        [System.ComponentModel.DisplayName("No")]
        public int Index
        {
            get; set;
        }

        [System.ComponentModel.DisplayName("CSV file")]
        public string CSV
        {
            get; set;
        }

        [System.ComponentModel.DisplayName("CSV state")]
        public string InputState
        {
            get; set;
        }

        [System.ComponentModel.DisplayName("Excel file")]
        public string Excel
        {
            get; set;
        }

        [System.ComponentModel.DisplayName("Excel state")]
        public string ExportState
        {
            get; set;
        }

        public override string ToString() => $"#{Index}: {InputState} | {ExportState} | {Excel}";
    }
}