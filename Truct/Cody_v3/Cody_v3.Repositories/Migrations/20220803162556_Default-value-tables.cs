using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cody_v3.Repositories.Migrations
{
    public partial class Defaultvaluetables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO dbo.Departments
                (
                    Id,
                    Level,
                    DepartmentCode,
                    DepartmentName,
                    DepartmentAddress,
                    PhoneNumber,
                    Email
                )	
                SELECT 'E4EC77E3-32A7-41FC-B0E9-64EF2EF448E5',2,'PM',N'PC Hải Dương',N'Số nhà 33, đại lộ Hồ Chí Minh, Phường Nguyễn Trãi, Thành phố Hải Dương, Tỉnh Hải Dương, VN',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '1DDBE36D-4D59-4FDF-9C95-990F0F720FAC',3,'PM0100',N'ĐL TP Hải Dương',N'Khu Cẩm Khê, phường Tứ Minh, TP Hải Dương, Tỉnh Hải Dương, Việt Nam',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '4D3E112B-147A-4F67-A9CC-AC51A4E6D520',3,'PM0300',N'ĐL Chí Linh',N'Số nhà 22 - Thái Học 2- Sao Đỏ - Chí Linh',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT 'B9C3463A-DB24-4EEE-A77E-7917F81EA9B8',3,'PM0500',N'ĐL Nam Sách',N'QL 37 Nguyễn Văn Trỗi - TT Nam Sách',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '6F89BC54-7330-4951-B7F2-AAED9CA8D0E4',3,'PM0700',N'ĐL Thanh Hà',N'Khu 1 - Thị trấn Thanh Hà',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '41BFD6FA-5157-4F58-81CA-C73C22AD09FB',3,'PM0900',N'ĐL Kinh Môn',N'số 281 Đ. Trần Hưng Đạo, Phường An Lưu, Thị xã Kinh Môn',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '34F4F90C-526B-4859-A3E8-86B47360C7A0',3,'PM1100',N'ĐL Kim Thành',N'Lai Khê - Cộng Hòa - Kim Thành - Hải Dương',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '8F8480AC-91B5-4CBF-A70D-2203B3B00497',3,'PM1300',N'ĐL Gia Lộc',N'Khu Ngã 3 - Thị Trấn Gia Lộc - Huyện Gia Lộc',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '9FCC5A53-F890-4D51-A2A8-E3F22E279155',3,'PM1500',N'ĐL Tứ Kỳ',N'Thị trấn Tứ Kỳ, huyện Tứ Kỳ, tỉnh Hải Dương',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '9A6B862E-F99C-48B3-9D05-737823DCADD7',3,'PM1700',N'ĐL Cẩm Giàng',N'TT Lai Cách - huyện Cẩm Giàng - HD.',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '24130F90-FD1A-42B9-9AD5-D28D2DBC8454',3,'PM1900',N'ĐL Bình Giang',N'Thôn My Thữ- Xã Vĩnh Hồng - Huyện Bình Giang - tỉnh Hải Dương',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '4B0F1A38-7A73-452F-80B2-6933ECA29875',3,'PM2100',N'ĐL Thanh Miện',N'Số 76,Đường Nguyễn Lương Bằng- TT Thanh Miện- H.Thanh Miện- HD',N'19006769',N'pchaiduong@npc.com.vn'
                union all SELECT '0654030F-28BD-4B41-A201-54DCFB323ED4',3,'PM2300',N'ĐL Ninh Giang',N'TT Ninh giang - huyện Ninh giang - HD',N'19006769',N'pchaiduong@npc.com.vn'

                GO      
                INSERT INTO dbo.Reclosers
                (
	                Id,
	                DepartmentId,
	                RecloserName,
	                RecloserQualify,
	                I_Norms,
	                LineType
                )		 
		        SELECT 'E445AC80-1E32-492B-8357-D675E877F325','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50100',N'Recloser 370/41',N'Recloser 370/41 đi nhánh 36 TNC - 370E8.1',N'231',N'Cáp AL150'
                union all SELECT 'CB12681D-02A3-4777-AA94-EA986EBED22E','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50100',N'Recloser 473/Cầu Hải Tân',N'Recloser 473/Cầu Hải Tân - 473E8.13',N'450',N'AC150'
                union all SELECT '572BAED6-AE8D-421C-84B0-678453A359C2','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50100',N'Recloser 474/25',N'Recloser 474/25 Tân Dân - 474E8.1',N'450',N'AC150'
                union all SELECT '96892DD8-FDD3-4B6F-993D-3A74FD398C3A','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50100',N'Recloser 478/30',N'Recloser 478/30 Triệu Quang Phục - 478E8.1',N'450',N'AC150'
                union all SELECT 'E6360192-9754-4AA3-9167-8F3DDF6F7B67','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50100',N'Recloser 478/65',N'Recloser 478/65 Đinh Văn Tả - 478E8.1',N'450',N'AC150'
                union all SELECT 'E4DE999B-6951-440F-BC43-FC4922B8FBC1','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50100',N'Recloser 480/15',N'Recloser 480/15 nhánh Phú Lương - 480E8.1',N'380',N'Dây bọc AL150'
                union all SELECT '92482D54-3787-4842-B01E-7B43CA0BA818','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50100',N'Recloser 480/32',N'Recloser 480/32 An Định - 480E8.1',N'450',N'AC150'
                union all SELECT '3778333E-767B-4CA4-90C8-A672BE2EA1C5','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50100',N'Recloser 480/Bình Hàn',N'Recloser 480/Bình Hàn - 480E8.1',N'450',N'AC150'
                union all SELECT 'CF0DACAE-3652-41EE-92B1-C90E667F59E6','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50100',N'Recloser 488/22A',N'Recloser 488/22A Trường Chinh - 488E8.1',N'450',N'AC150'
                union all SELECT 'EE345A51-5CAF-4E7E-BB3A-BB1E9A062D36','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser 371/01',N'Recloser 371/01 nhánh Đội 8 - 371E8.5',N'210',N'AC50'
                union all SELECT '98B6E662-B576-488F-B26E-E39099534990','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser 371/39',N'Recloser 371/39A Đại Tân - 371E8.5',N'330',N'AC95'
                union all SELECT '1ACD5730-EC53-45C4-8D51-9E5764DEBE60','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser 472/01',N'Recloser 472/01 nhánh Đá Cóc - 472E8.5',N'210',N'AC50'
                union all SELECT '545D2038-357F-4EBC-9493-2B599A098A23','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser 474/54',N'Recloser 474/54 - 474E8.5',N'265',N'AC70'
                union all SELECT '4DE27D64-2924-43C2-9B6A-31F4C4F48D0D','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser 476/04',N'Recloser 476/04 nhánh Đơn vị 490 - 476E8.5',N'210',N'AC50'
                union all SELECT '13EE3E5B-CC4F-44DA-B9FA-13C3C49B3EB1','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser 476/07',N'Recloser 476/07 nhánh Công Đoàn - 476E8.5',N'210',N'AC50'
                union all SELECT '1DA58E31-B355-4394-97E8-E6323828D3C0','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser 54',N'Recloser 54 - 472E8.5',N'210',N'AC50'
                union all SELECT 'C609BB11-56CE-4081-8DA4-7B78AFBF779C','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser 69A',N'Recloser 69A - 371E8.5',N'330',N'AC95'
                union all SELECT '5141EF90-124F-4A7A-98DF-8B86D985CE0D','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser 72',N'Recloser 72 - 478E8.5',N'330',N'AC95'
                union all SELECT '9C0434ED-BBDF-4D62-BBC0-AF4AA8C52E84','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser Mỏ Than',N'Recloser Mỏ Than - 474E8.5',N'265',N'AC70'
                union all SELECT '391F631E-2F9A-4A49-88E2-7A68889A63FB','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50300',N'Recloser Nông Trường',N'Recloser Nông Trường - 371E8.5',N'265',N'AC70'
                union all SELECT '62C6366C-4756-4C24-A484-EC43A757CA08','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51700',N'Recloser 371/01',N'Recloser 371/01 nhánh Kênh Vàng - 371E8.11',N'330',N'AC95'
                union all SELECT '7A89C920-DDCF-4D6B-9446-18763A192FFA','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51700',N'Recloser 372/02',N'Recloser 372/02 nhánh KCN Tân Trường - 372E8.15',N'390',N'AC120'
                union all SELECT 'F0AD91B3-34F7-4E07-A6CE-41F644D6F112','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51700',N'Recloser 374/73',N'Recloser 374/73 - 374E8.1',N'390',N'AC120'
                union all SELECT 'CAE9AEE7-C774-4A26-85AB-823C863D1B94','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51700',N'Recloser 377/14',N'Recloser 377/14 nhánh Dự án nước - 377E8.21',N'330',N'AC95'
                union all SELECT '4F46A09A-D5D2-4B9D-B621-C0A489DF0193','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51700',N'Recloser 377/54',N'Recloser 377/54 Quán Gỏi - 377E8.21',N'330',N'AC95'
                union all SELECT '986513DD-4DA8-4861-9DBC-A517334ABA24','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51700',N'Recloser 480/15',N'Recloser 480/15 - 480E8.11',N'450',N'AC150'
                union all SELECT '74AC3557-7CB5-46B1-B1A6-10B596921C9D','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51900',N'Recloser 373/10',N'Recloser 373/10 nhánh Hùng Thắng - 373E8.11',N'265',N'AC70'
                union all SELECT 'ABAA97D7-97E1-4D7A-93F9-84489A569324','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51900',N'Recloser 375/98A',N'Recloser 375/98A - 375E8.15',N'330',N'AC95'
                union all SELECT '41CA3B30-1B10-4849-94D6-913E942857C9','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51900',N'Recloser 377/01',N'Recloser 377/01 nhánh Thúc Kháng - 377E8.15',N'265',N'AC70'
                union all SELECT 'ECB72801-8964-4634-BEB1-267581F30EE6','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51900',N'Recloser 68',N'Recloser 68 - 371E28.12',N'330',N'AC95'
                union all SELECT 'DF21E2C1-F70D-4CBB-870F-72062763CB4C','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51900',N'Recloser Bình Xuyên',N'Recloser Bình Xuyên - 373E8.14',N'265',N'AC70'
                union all SELECT '9DE56B70-E114-4181-A83B-0727A78C8D28','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51900',N'Recloser Hồng Khê',N'Recloser Hồng Khê - 373E8.14',N'265',N'AC70'
                union all SELECT '15CEEA65-CE87-49D2-AB70-BC65C266ADCF','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51900',N'Recloser Tráng Liệt',N'Recloser Tráng Liệt - 375E8.15',N'210',N'AC50'
                union all SELECT '38E1332F-086F-43F7-A01C-DABE745936FA','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50500',N'Recloser 371/07',N'Recloser 371/07 nhánh Nam Đồng - 371E8.16',N'330',N'AC95'
                union all SELECT 'BFBF2855-B469-4ED9-8DD4-0CBAB430C517','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50500',N'Recloser 375/02',N'Recloser 375/02 nhánh Hiệp Cát - 375E8.5',N'265',N'AC70'
                union all SELECT 'C68D751E-7ED4-4B81-BE79-E984338790E3','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50500',N'Recloser 375/63A',N'Recloser 375/63A - 375E8.5',N'390',N'AC120'
                union all SELECT 'D4B5A80D-C286-40FA-8A78-D5A328277207','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50500',N'Recloser 50',N'Recloser 50 - 371E8.16',N'330',N'AC95'
                union all SELECT 'DB2BE6B7-D5A3-4D84-9FF5-0AECB69BFBF2','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50500',N'Recloser Chu Đậu',N'Recloser Chu Đậu - 372E8.16',N'265',N'AC70'
                union all SELECT '317B2E62-53D6-4087-9A08-8411536495E8','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50500',N'Recloser Thanh Quang',N'Recloser Thanh Quang - 372E8.16',N'265',N'AC70'
                union all SELECT 'D36B525A-7744-4198-9D18-F07FC9DBC730','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50700',N'Recloser 373/07',N'Recloser 373/07 nhánh bơm Thanh Cường - 373E8.13',N'210',N'AC50'
                union all SELECT 'C1139D5F-373E-4FE6-B1B4-C7AB1A8BA7F2','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50700',N'Recloser 373/102A',N'Recloser 373/102A nhánh Cấp Tứ - 373E8.6',N'265',N'AC70'
                union all SELECT 'B922533A-6F70-4131-8987-4AF4E26380DE','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50700',N'Recloser 38',N'Recloser 38 - 373E8.6',N'265',N'AC70'
                union all SELECT 'CE233752-C46A-4BFB-AAB8-9770201375B7','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50700',N'Recloser 5A Quyết Thắng',N'Recloser 5A Quyết Thắng - 372E8.13',N'210',N'AC50'
                union all SELECT 'DAB9EC76-1716-4B08-8176-E77515D7F49F','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50700',N'Recloser 68',N'Recloser 68 - 372E8.13',N'450',N'AC150'
                union all SELECT 'DBE0C313-29E7-4515-A187-F2BA9F3250AB','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser Mỏ đá Thống Nhất',N'Rec Mỏ đá Thống Nhất - 370E8.10',N'265',N'AC70'
                union all SELECT 'C7C3F5D3-2A74-486E-8AA5-D08AC78D52C7','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser 371/162',N'Recloser 371/162 - 371E2.11',N'330',N'AC95'
                union all SELECT '18E94239-F077-4266-84BD-E4F5FA753DD1','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser 371/1A',N'Recloser 371/1A nhánh bơm An Phụ - 371E8.6',N'265',N'AC70'
                union all SELECT '2D2DE5DE-4067-4F96-B837-A62DC53E5D8A','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser 373/01',N'Recloser 373/01 nhánh Phạm Mệnh A - 373E8.10',N'210',N'AC50'
                union all SELECT 'A1ACFAAC-11FC-4C6E-BBCD-26722BC56AD9','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser 373/46',N'Recloser 373/46 Hiệp Sơn - 373E8.10',N'330',N'AC95'
                union all SELECT 'AABFE4AC-E658-46D6-923E-527B43006399','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser 373/93',N'Recloser 373/93 Phạm Thái - 373E8.10',N'330',N'AC95'
                union all SELECT '277506FC-D994-4453-A699-E592FC2B928A','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser 375/74A',N'Recloser 375/74A An Lưu - 375E8.10',N'265',N'AC70'
                union all SELECT 'A175B16E-7F97-456A-832F-C6BCFD8442F9','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser 377/89A',N'Recloser 377/89A Hoành Sơn - 377E8.10',N'330',N'AC95'
                union all SELECT 'ABE9D406-4DB4-4C25-872F-64C7F2ED7286','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser 385/02',N'Recloser 385/02 nhánh Hạ Chiểu - 385E8.10',N'265',N'AC70'
                union all SELECT '64CA1CDD-69AA-4416-BC9E-DC757FC946C0','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser 38A',N'Recloser 38A - 377E8.10',N'330',N'AC95'
                union all SELECT 'CAC49A71-2079-4525-B118-DFDD27721CF9','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser 79B',N'Recloser 79B - 371E8.6',N'330',N'AC95'
                union all SELECT '9A013559-B3B3-4D9E-AA87-4EAB7B853115','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser Hiến Thành',N'Recloser Hiến Thành - 375E8.10',N'265',N'AC70'
                union all SELECT 'C94DD1C4-687B-4A07-9609-C62BB6800EF6','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E50900',N'Recloser Kênh Than',N'Recloser Kênh Than - 371E8.6',N'265',N'AC70'
                union all SELECT '53038AC5-5D04-4B76-B09B-CA354B0F647F','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51100',N'Recloser 373/03',N'Recloser 373/03 nhánh BCH - 373E8.22',N'510',N'AC185'
                union all SELECT 'CD1330A3-AC95-4ADC-8241-55C8D26B0FFB','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51100',N'Recloser 374/01',N'Recloser 374/01 nhánh bơm Kim Xuyên - 374E8.6',N'265',N'AC70'
                union all SELECT 'A1A73B82-B417-4712-9451-D65AB5E1734F','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51100',N'Recloser 374/02',N'Recloser 374/02 nhánh NM rác thải - 374E8.6',N'210',N'AC50'
                union all SELECT 'E07FF3D8-0C18-4D39-B867-9A2BA8D75C48','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51100',N'Recloser 375/54',N'Recloser 375/54 Kim Đính - 375E8.22',N'450',N'AC150'
                union all SELECT '596AB63E-DE1F-464C-94C5-8FF44D5B90B8','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51100',N'Recloser 377/04',N'Recloser 377/04 nhánh bơm Đại Đức - 377E8.22',N'210',N'AC50'
                union all SELECT 'F142D148-CBB7-4B4F-A9D2-035D42BE927F','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51100',N'Recloser 92',N'Recloser 92 - 375E8.6',N'330',N'AC95'
                union all SELECT '4962876A-5A17-453A-A31F-52A6426FEC86','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51500',N'Recloser 371/01',N'Recloser 371/01 nhánh Văn Tố - 371E8.19',N'210',N'AC50'
                union all SELECT 'CA8A3F12-179F-4A13-8032-914830337AD2','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51500',N'Recloser 373/03',N'Recloser 373/03 nhánh Thanh Cường - 373E8.13',N'265',N'AC70'
                union all SELECT 'CEC0432E-2C19-4376-B552-B7A36D64EC09','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51500',N'Recloser 373/95',N'Recloser 373/95 Hưng Đạo - 373E8.13',N'390',N'AC120'
                union all SELECT 'FEB42BC6-2F97-43B4-AFB4-3964F4C3B252','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51500',N'Recloser 375/24',N'Recloser 375/24 nhánh Triều Trái - 375E8.7',N'210',N'AC50'
                union all SELECT 'BE94E706-9C67-4057-A7C9-8585650E3079','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51500',N'Recloser 375/52',N'Recloser 375/52 Nguyên Giáp - 375E8.19',N'210',N'AC50'
                union all SELECT '7DE54D93-B027-4898-A6AF-75E2C26FA7E0','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E51500',N'Recloser 376/110',N'Recloser 376/110 nhánh Minh Đức - 376E8.7',N'390',N'AC120'
                union all SELECT '7FFA1F06-7C08-4F8B-82E3-D654633D3CE1','8F8480AC-91B5-4CBF-A70D-2203B3B00497',N'Recloser 370/02',N'Recloser 370/02 nhánh CCN Gia Lộc - 370E8.1',N'265',N'AC70'
                union all SELECT '143D7DDA-6A71-4DB7-A8FF-3BB370726FB4','8F8480AC-91B5-4CBF-A70D-2203B3B00497',N'Recloser 373/68',N'Recloser 373/68 Cầu Bía - 373E8.7',N'330',N'AC95'
                union all SELECT '8233D04D-2F16-4213-A348-522D8AF38E31','8F8480AC-91B5-4CBF-A70D-2203B3B00497',N'Recloser 377/01',N'Recloser 377/01 nhánh Quảng Nghiệp - 377E8.14',N'330',N'AC95'
                union all SELECT '74EEC9F5-89BF-4586-9C95-50E8099596F2','8F8480AC-91B5-4CBF-A70D-2203B3B00497',N'Recloser 377/124',N'Recloser 377/124 - 377E8.14',N'450',N'AC150'
                union all SELECT 'CC57BD1A-DAD6-4C72-A6D5-81B44970032F','8F8480AC-91B5-4CBF-A70D-2203B3B00497',N'Recloser 378/126',N'Recloser 378/126 - 378E8.1',N'390',N'AC120'
                union all SELECT '5CF2563C-3DB7-4D7A-94FB-5CC1813149D7','8F8480AC-91B5-4CBF-A70D-2203B3B00497',N'Recloser 378/13',N'Recloser 378/13 nhánh Khuông Phụ - 378E8.1',N'210',N'AC50'
                union all SELECT '590B7DA9-F2B2-4E16-8B5E-F85F821412CA','0654030F-28BD-4B41-A201-54DCFB323ED4',N'Recloser 370/136',N'Recloser 370/136 Cống Lê - 370E8.7',N'265',N'AC70'
                union all SELECT '2ACA6C9C-AF89-40EA-AE0F-D928892D6403','0654030F-28BD-4B41-A201-54DCFB323ED4',N'Recloser 372/05',N'Recloser 372/05 nhánh Quang Hưng - 372E8.7',N'330',N'AC95'
                union all SELECT 'C7E575CE-9898-42BC-89F9-CB76546D1AFA','0654030F-28BD-4B41-A201-54DCFB323ED4',N'Recloser 375/67',N'Recloser 375/67 - 375E8.7',N'390',N'AC120'
                union all SELECT '27249160-726C-4625-9651-3B1DF7F61F12','0654030F-28BD-4B41-A201-54DCFB323ED4',N'Recloser 377/126',N'Recloser 377/126 Quang Rực - 377E8.7',N'265',N'AC70'
                union all SELECT '701FBF79-D4F7-4784-A710-D7F8A8B811C3','0654030F-28BD-4B41-A201-54DCFB323ED4',N'Recloser 377/46',N'Recloser 377/46 Đông Xuyên - 377E8.7',N'265',N'AC70'
                union all SELECT '99D6F977-E37F-498D-9E1D-5A65FA9BF680','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E52100',N'Recloser 373/74',N'Recloser 373/74 - 373E8.14',N'330',N'AC95'
                union all SELECT '672ADE42-E647-4E53-9284-7AAD0AD93CE4','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E52100',N'Recloser 476/5A',N'Recloser 476/5A nhánh Hoàng Tường - 476E8.14',N'265',N'AC70'
                union all SELECT '7DCF2E1F-F2EE-4015-B218-07DF11B6B308','E4EC77E3-32A7-41FC-B0E9-64EF2EF448E52100',N'Recloser Phạm Kha',N'Recloser Phạm Kha - 472E8.14',N'265',N'AC70'
                GO 
                ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            delete from dbo.Reclosers WHERE 0=0
            delete dbo.Departments WHERE 0=0
            ");
        }
    }
}
