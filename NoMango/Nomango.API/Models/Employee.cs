using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nomango.API.Models
{
    public class Employee
    {
        public static List<Employee> Employees = new List<Employee>()
        {
            new Employee() {EmployeeCode="NV001", EmployeeName="Lê Văn Tùng",Email="TungLV@gmail.com",Mobile="01920129",CompanyName="ITG",Address="Hà Nội"},
            new Employee() {EmployeeCode="NV002", EmployeeName="Bùi Thị Thảo",Email="ThảoLV@gmail.com",Mobile="01920129",CompanyName="ITG",Address="Hà Tây"},
            new Employee() {EmployeeCode="NV003", EmployeeName="Phạm thị Nhung",Email="NhungLV@gmail.com",Mobile="01920129",CompanyName="ITG",Address="Quốc Oai"},
            new Employee() {EmployeeCode="NV004", EmployeeName="Nguyễn Thị Vui",Email="AnhLV@gmail.com",Mobile="01920129",CompanyName="ITG",Address="HCM"},
            new Employee() {EmployeeCode="NV005", EmployeeName="Bùi Thị Tươi",Email="Minh@gmail.com",Mobile="01920129",CompanyName="ITG",Address="Bình dương"}
        };
        
        /// <summary>
    /// Mã nhân viên
    /// </summary>
        public string EmployeeCode {  get; set; }
        /// <summary>
        /// mã nhân viên
        /// </summary>
        public string EmployeeName { get; set; }
        /// <summary>
        /// email
        /// </summary>
        public string Email {  get; set; }  
        /// <summary>
        /// điện thoại
        /// </summary>
        public string Mobile {  get; set; } 
        /// <summary>
        /// tên công ty
        /// </summary>
        public string CompanyName {  get; set; }
        /// <summary>
        /// địa chỉ
        /// </summary>
        public string Address { get; set; }
    }
}