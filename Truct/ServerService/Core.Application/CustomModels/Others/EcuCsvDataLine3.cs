using Core.Application.CustomModels.Dtos;
using CsvHelper.Configuration.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Core.Application.CustomModels.Others
{
    /// <summary>
    /// Thông tin ECU get được từ file Csv
    /// </summary>
    public class EcuCsvDataLine3
    {
        [Description("Ngày giờ")]
        [Index(0)]//Index column in file CSv
        public DateTime? DateManufacture { get; set; }

        [Index(1)]
        public string Model { get; set; }

        [Index(2)]
        [Description("Mã NG")]
        public string NgCode { get; set; }//EnumCommon. 0 is NG, 1 is OK

        [Description("Kết quả")]
        [Index(3)]
        public string Result { get; set; }

        [Index(4)]
        [Description("Mã HU")]
        public string? HUCode { get; set; }
        [Index(5)]
        public string CustomerPartNumber { get; set; }
        [Index(6)]
        public string PCBCode { get; set; }
        [Index(7)]
        public string EcuId { get; set; }
        [Index(8)]
        public string ErrorCode { get; set; }
        [Index(9)]
        public string EcuConfirmation1 { get; set; }
        [Index(10)]
        public string MotorDrive { get; set; }
        [Index(11)]
        public string SCS { get; set; }
        [Index(12)]
        public string BLS { get; set; }
        [Index(13)]
        public string ABSWL { get; set; }
        [Index(14)]
        public string WheelSpeed { get; set; }
        [Index(15)]
        public string BUSY { get; set; }
        [Index(16)]
        public string EXI { get; set; }
        [Index(17)]
        public string MODESEL { get; set; }
        [Index(18)]
        public string SPD { get; set; }
        [Index(19)]
        public string ESS { get; set; }
        [Index(20)]
        public string EcuConfirmation2 { get; set; }
        [Index(21)]
        public string ScsOn { get; set; }
        [Index(22)]
        public string ScsOff { get; set; }
        [Index(23)]
        public string BlsOn { get; set; }
        [Index(24)]
        public string BlsOff { get; set; }
        [Index(25)]
        public string AbsWlOn { get; set; }
        [Index(26)]
        public string AbsWlOff { get; set; }
        [Index(27)]
        public string MotorCurrent { get; set; }
        [Index(28)]
        public string SoFrWheelSpeed { get; set; }
        [Index(29)]
        public string ExiFrWheelSpeed { get; set; }
        [Index(30)]
        public string SoRrWheelSpeed { get; set; }
        [Index(31)]
        public string ExiRrWheelSpeed { get; set; }
        [Index(32)]
        public string SoBusyOffVoltage { get; set; }
        [Index(33)]
        public string SoBusyOnVoltage { get; set; }
        [Index(34)]
        public string ExiExiOnVoltage { get; set; }
        [Index(35)]
        public string ExiExiOffVoltage { get; set; }
        [Index(36)]
        public string CanlModeselOnVoltage { get; set; }
        [Index(37)]
        public string CanlModeselOffVoltage { get; set; }
        [Index(38)]
        public string ExilModeselOnVoltage { get; set; }
        [Index(39)]
        public string ExiModeselOffVoltage { get; set; }
        [Index(40)]
        public string ModeselModeselOnVoltage { get; set; }
        [Index(41)]
        public string ModeselModeselOffVoltage { get; set; }
        [Index(42)]
        public string SpdInputOn { get; set; }
        [Index(43)]
        public string SpdInputOff { get; set; }
        [Index(44)]
        public string SpecialMissionFlag { get; set; }
        [Index(45)]
        public string ErrorCode2 { get; set; }
        [Index(46)]
        public string EcuVoltage { get; set; }
        [Index(47)]
        public string ProcessControlCode { get; set; }
        [Index(48)]
        public string BoardSerial { get; set; }
        [Index(49)]
        public string CarMaker { get; set; }
        [Index(50)]
        public string ErrorBitmap { get; set; }
        [Index(51)]
        public string NgCode2 { get; set; }
        [Index(52)]
        public string SoftwareName { get; set; }
        [Index(53)]
        public string EssOn { get; set; }
        [Index(54)]
        public string EssOff { get; set; }
        [Index(55)]
        public string CT { get; set; }
        [Index(56)]
        public string ModelNumber { get; set; }
        [Index(57)]
        [Description("In laze")]
        public string LaserPrinting { get; set; }
    }
}
