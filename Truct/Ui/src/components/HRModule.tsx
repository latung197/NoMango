import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Separator } from './ui/separator';
import { 
  Users,
  UserPlus,
  Building2,
  Clock,
  CalendarDays,
  ClipboardCheck,
  FileText,
  ArrowLeft
} from 'lucide-react';
import { StaffRegistration } from './hr/StaffRegistration';
import { DepartmentRegistration } from './hr/DepartmentRegistration';
import { ShiftFlexibleSetup } from './hr/ShiftFlexibleSetup';
import { ShiftWorkManagement } from './hr/ShiftWorkManagement';
import { AttendanceRecord } from './hr/AttendanceRecord';
import { HRReports } from './hr/HRReports';

interface HRModuleProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

export function HRModule({ currentPage, onPageChange }: HRModuleProps) {
  // Sub-navigation items
  const subPages = [
    {
      id: 'staff-registration',
      name: 'Staff Registration',
      nameLocal: 'ဝန်ထမ်းမှတ်ပုံတင်',
      icon: UserPlus,
      description: 'Register new employees with RFID and work setup',
      descriptionLocal: 'RFID နှင့် အလုပ်အစီအစဉ်နှင့်အတူ ဝန်ထမ်းအသစ်များမှတ်ပုံတင်ခြင်း',
      color: 'bg-blue-600',
      stats: { total: 124, active: 118, inactive: 6 }
    },
    {
      id: 'department-registration',
      name: 'Department Registration',
      nameLocal: 'ဌာနစာရင်းသွင်း',
      icon: Building2,
      description: 'Manage departments and organizational structure',
      descriptionLocal: 'ဌာနများနှင့် အဖွဲ့အစည်းဖွဲ့စည်းပုံကို စီမံခန့်ခွဲခြင်း',
      color: 'bg-green-600',
      stats: { total: 8, active: 8, staff: 124 }
    },
    {
      id: 'shift-flexible-setup',
      name: 'Shift & Flexible Setup',
      nameLocal: 'Shift နှင့် အချိန်ပိုင်းအလုပ် စာရင်းသွင်း',
      icon: Clock,
      description: 'Configure shift schedules and flexible work arrangements',
      descriptionLocal: 'အလုပ်ပတ်အစီအစဉ်နှင့် လိုက်လျောညီထွေ အလုပ်အစီအစဉ်များ ပြင်ဆင်ခြင်း',
      color: 'bg-purple-600',
      stats: { shifts: 3, flexible: 12 }
    },
    {
      id: 'shift-work-management',
      name: 'Shift & Work Management',
      nameLocal: 'အလုပ်ချိန်စီမံခန့်ခွဲမှု',
      icon: CalendarDays,
      description: 'Weekly/monthly shift assignment and planning',
      descriptionLocal: 'အပတ်စဉ်/လစဉ် အလုပ်ပတ်ခန့်အပ်မှုနှင့် အစီအစဉ်ချမှတ်ခြင်း',
      color: 'bg-orange-600',
      stats: { assigned: 95, conflicts: 2 }
    },
    {
      id: 'attendance-record',
      name: 'Attendance Record',
      nameLocal: 'တက်ရောက်မှုမှတ်တမ်း',
      icon: ClipboardCheck,
      description: 'Track daily attendance and working hours',
      descriptionLocal: 'နေ့စဉ်တက်ရောက်မှုနှင့် အလုပ်ချိန်များကို ခြေရာခံခြင်း',
      color: 'bg-indigo-600',
      stats: { present: 89, late: 5, absent: 2 }
    },
    {
      id: 'hr-reports',
      name: 'Reports',
      nameLocal: 'အစီရင်ခံစာများ',
      icon: FileText,
      description: 'Comprehensive HR analytics and reports',
      descriptionLocal: 'ပြည့်စုံသော HR ခွဲခြမ်းစိတ်ဖြာမှုနှင့် အစီရင်ခံစာများ',
      color: 'bg-pink-600',
      stats: { reports: 6, automated: 4 }
    }
  ];

  const currentSubPage = subPages.find(page => page.id === currentPage);

  // If we're in a specific sub-page, render that component
  if (currentPage === 'staff-registration') {
    return <StaffRegistration />;
  }
  
  if (currentPage === 'department-registration') {
    return <DepartmentRegistration />;
  }
  
  if (currentPage === 'shift-flexible-setup') {
    return <ShiftFlexibleSetup />;
  }
  
  if (currentPage === 'shift-work-management') {
    return <ShiftWorkManagement />;
  }
  
  if (currentPage === 'attendance-record') {
    return <AttendanceRecord />;
  }
  
  if (currentPage === 'hr-reports') {
    return <HRReports />;
  }

  // Main HR Module Dashboard
  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="mb-6">
          <div className="flex items-center gap-3 mb-4">
            <div className="p-2 bg-blue-100 rounded-lg">
              <Users className="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <h1 className="text-3xl font-semibold text-slate-900">
                HR Management | လူစာရင်းစီမံခန့်ခွဲမှု
              </h1>
              <p className="text-slate-600 mt-1">
                Human Resources and staff management system
              </p>
              <p className="text-sm text-slate-500">
                လူ့စွမ်းအားနှင့် ဝန်ထမ်းစီမံခန့်ခွဲမှုစနစ်
              </p>
            </div>
          </div>
        </div>

        {/* Quick Stats Dashboard */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-green-100 rounded-lg">
                  <div className="w-3 h-3 bg-green-500 rounded-full"></div>
                </div>
                <div>
                  <div className="text-2xl font-bold text-green-700">89</div>
                  <div className="text-sm text-slate-600">Present Today</div>
                  <div className="text-xs text-slate-500">ယနေ့တက်ရောက်သူများ</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-yellow-100 rounded-lg">
                  <div className="w-3 h-3 bg-yellow-500 rounded-full"></div>
                </div>
                <div>
                  <div className="text-2xl font-bold text-yellow-700">5</div>
                  <div className="text-sm text-slate-600">Late Arrivals</div>
                  <div className="text-xs text-slate-500">နောက်ကျရောက်သူများ</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-red-100 rounded-lg">
                  <div className="w-3 h-3 bg-red-500 rounded-full"></div>
                </div>
                <div>
                  <div className="text-2xl font-bold text-red-700">2</div>
                  <div className="text-sm text-slate-600">Absent</div>
                  <div className="text-xs text-slate-500">မရှိသူများ</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <Users className="w-4 h-4 text-blue-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-blue-700">124</div>
                  <div className="text-sm text-slate-600">Total Staff</div>
                  <div className="text-xs text-slate-500">စုစုပေါင်းဝန်ထမ်း</div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

        {/* Who is in the factory now widget */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <div className="p-2 bg-green-100 rounded-lg">
                <ClipboardCheck className="h-5 w-5 text-green-600" />
              </div>
              <div>
                <div>Who is in the factory now? | စက်မှုလုပ်ငန်းတွင် ယခုရှိနေသူများ</div>
                <div className="text-sm text-slate-500">Real-time staff presence overview</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              {/* Shift-based Staff */}
              <div>
                <h4 className="text-base font-medium text-slate-900 mb-3">
                  Shift-based Staff | အလုပ်ပတ်အခြေခံဝန်ထမ်းများ
                </h4>
                <div className="space-y-3">
                  {[
                    { shift: 'Morning Shift (08:00-17:00)', count: 45, status: 'active' },
                    { shift: 'Evening Shift (17:00-02:00)', count: 32, status: 'active' },
                    { shift: 'Night Shift (02:00-08:00)', count: 0, status: 'inactive' }
                  ].map((shift, index) => (
                    <div key={index} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                      <div>
                        <div className="font-medium text-slate-900">{shift.shift}</div>
                        <div className="text-sm text-slate-500">
                          {shift.shift === 'Morning Shift (08:00-17:00)' ? 'နံနက်ပတ် (၀၈:၀၀-၁၇:၀၀)' :
                           shift.shift === 'Evening Shift (17:00-02:00)' ? 'ညနေပတ် (၁၇:၀၀-၀၂:၀၀)' :
                           'ညပတ် (၀၂:၀၀-၀၈:၀၀)'}
                        </div>
                      </div>
                      <Badge className={`${shift.status === 'active' ? 'bg-green-100 text-green-800' : 'bg-slate-100 text-slate-600'}`}>
                        {shift.count} Staff
                      </Badge>
                    </div>
                  ))}
                </div>
              </div>

              {/* Flexible Staff */}
              <div>
                <h4 className="text-base font-medium text-slate-900 mb-3">
                  Flexible Staff | လိုက်လျောညီထွေဝန်ထမ်းများ
                </h4>
                <div className="space-y-3">
                  {[
                    { dept: 'Engineering', present: 8, total: 12 },
                    { dept: 'Quality Control', present: 3, total: 4 },
                    { dept: 'Administration', present: 4, total: 6 }
                  ].map((dept, index) => (
                    <div key={index} className="flex items-center justify-between p-3 bg-slate-50 rounded-lg">
                      <div>
                        <div className="font-medium text-slate-900">{dept.dept}</div>
                        <div className="text-sm text-slate-500">
                          {dept.dept === 'Engineering' ? 'အင်ဂျင်နီယာ' :
                           dept.dept === 'Quality Control' ? 'အရည်အသွေးထိန်းချုပ်မှု' :
                           'စီမံခန့်ခွဲမှု'}
                        </div>
                      </div>
                      <Badge className="bg-blue-100 text-blue-800">
                        {dept.present}/{dept.total} Present
                      </Badge>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Sub-modules Grid */}
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
          {subPages.map((page) => {
            const IconComponent = page.icon;
            return (
              <Card key={page.id} className="border-0 shadow-sm hover:shadow-md transition-shadow">
                <CardHeader className="pb-4">
                  <CardTitle className="flex items-center gap-3">
                    <div className={`p-3 ${page.color} rounded-lg`}>
                      <IconComponent className="h-6 w-6 text-white" />
                    </div>
                    <div>
                      <div className="text-xl font-semibold text-slate-900">{page.name}</div>
                      <div className="text-base text-slate-600">{page.nameLocal}</div>
                    </div>
                  </CardTitle>
                </CardHeader>
                <CardContent className="pt-0">
                  <div className="mb-4">
                    <p className="text-slate-700 mb-1">{page.description}</p>
                    <p className="text-sm text-slate-500">{page.descriptionLocal}</p>
                  </div>

                  {/* Module-specific stats */}
                  <div className="mb-4 space-y-2">
                    {page.id === 'staff-registration' && (
                      <>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Total Staff:</span>
                          <Badge className="bg-blue-100 text-blue-800">{page.stats.total}</Badge>
                        </div>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Active:</span>
                          <Badge className="bg-green-100 text-green-800">{page.stats.active}</Badge>
                        </div>
                      </>
                    )}

                    {page.id === 'department-registration' && (
                      <>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Departments:</span>
                          <Badge className="bg-green-100 text-green-800">{page.stats.total}</Badge>
                        </div>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Total Staff:</span>
                          <Badge className="bg-blue-100 text-blue-800">{page.stats.staff}</Badge>
                        </div>
                      </>
                    )}

                    {page.id === 'shift-flexible-setup' && (
                      <>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Shift Types:</span>
                          <Badge className="bg-purple-100 text-purple-800">{page.stats.shifts}</Badge>
                        </div>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Flexible Staff:</span>
                          <Badge className="bg-blue-100 text-blue-800">{page.stats.flexible}</Badge>
                        </div>
                      </>
                    )}

                    {page.id === 'shift-work-management' && (
                      <>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Assigned Today:</span>
                          <Badge className="bg-green-100 text-green-800">{page.stats.assigned}</Badge>
                        </div>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Conflicts:</span>
                          <Badge className="bg-red-100 text-red-800">{page.stats.conflicts}</Badge>
                        </div>
                      </>
                    )}

                    {page.id === 'attendance-record' && (
                      <>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Present:</span>
                          <Badge className="bg-green-100 text-green-800">{page.stats.present}</Badge>
                        </div>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Late/Absent:</span>
                          <Badge className="bg-yellow-100 text-yellow-800">{page.stats.late + page.stats.absent}</Badge>
                        </div>
                      </>
                    )}

                    {page.id === 'hr-reports' && (
                      <>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Available Reports:</span>
                          <Badge className="bg-pink-100 text-pink-800">{page.stats.reports}</Badge>
                        </div>
                        <div className="flex items-center justify-between text-sm">
                          <span className="text-slate-600">Automated:</span>
                          <Badge className="bg-green-100 text-green-800">{page.stats.automated}</Badge>
                        </div>
                      </>
                    )}
                  </div>

                  <Button 
                    onClick={() => onPageChange(page.id)}
                    className={`w-full h-12 ${page.color} hover:opacity-90 transition-opacity`}
                  >
                    <span className="mr-2">Open Module</span>
                    <span className="text-sm opacity-80">| မော်ဂျူးဖွင့်ရန်</span>
                  </Button>
                </CardContent>
              </Card>
            );
          })}
        </div>

        {/* Recent HR Activity */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <div className="p-2 bg-slate-100 rounded-lg">
                <ClipboardCheck className="h-5 w-5 text-slate-600" />
              </div>
              <div>
                <div>Recent HR Activity</div>
                <div className="text-sm text-slate-500">လတ်တလော HR လှုပ်ရှားမှုများ</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {[
                {
                  time: '09:15',
                  event: 'Ko Zaw (EMP-045) clocked in - Morning Shift',
                  eventLocal: 'ကိုဇော် (EMP-045) ၀င်ချိန်မှတ်တမ်း - နံနက်ပတ်',
                  type: 'success'
                },
                {
                  time: '09:10',
                  event: 'Ma Aye (EMP-032) marked late - 10 minutes',
                  eventLocal: 'မအေး (EMP-032) နောက်ကျမှတ်တမ်း - ၁၀ မိနစ်',
                  type: 'warning'
                },
                {
                  time: '08:45',
                  event: 'New employee Ko Min (EMP-125) registered',
                  eventLocal: 'ဝန်ထမ်းအသစ် ကိုမင်း (EMP-125) မှတ်ပုံတင်ပြီး',
                  type: 'info'
                },
                {
                  time: '08:30',
                  event: 'Department "Quality Assurance" updated - 2 new positions',
                  eventLocal: 'ဌာန "အရည်အသွေးအာမခံ" အပ်ဒေ့ - ရာထူးအသစ် ၂ ခု',
                  type: 'info'
                },
                {
                  time: '08:15',
                  event: 'Shift assignment completed for this week',
                  eventLocal: 'ဤအပတ်အတွက် အလုပ်ပတ်ခန့်အပ်မှု ပြီးစီးပြီ',
                  type: 'success'
                }
              ].map((activity, index) => (
                <div key={index} className="flex items-start gap-3 p-3 bg-slate-50 rounded-lg">
                  <div className={`w-2 h-2 rounded-full mt-2 ${
                    activity.type === 'success' ? 'bg-green-500' :
                    activity.type === 'warning' ? 'bg-yellow-500' :
                    'bg-blue-500'
                  }`}></div>
                  <div className="flex-1">
                    <div className="flex items-center gap-2 mb-1">
                      <span className="text-sm font-mono text-slate-500">{activity.time}</span>
                      <span className="text-sm text-slate-700">{activity.event}</span>
                    </div>
                    <p className="text-xs text-slate-500">{activity.eventLocal}</p>
                  </div>
                </div>
              ))}
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}