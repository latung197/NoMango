import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Separator } from './ui/separator';
import { 
  Factory,
  Monitor,
  UserCheck,
  Eye,
  RefreshCw,
  ArrowLeft,
  Droplets,
  Users
} from 'lucide-react';
import { OperatorKiosk } from './OperatorKiosk';
import { SupervisorScreen } from './SupervisorScreen';
import { LiveMonitoring } from './LiveMonitoring';
import { OperatorDashboard } from './OperatorDashboard';

import GlueFillOperator from './GlueFillOperator';

interface ProductionModuleProps {
  currentPage: string;
  onPageChange: (page: string) => void;
}

export function ProductionModule({ currentPage, onPageChange }: ProductionModuleProps) {
  const [glueFillView, setGlueFillView] = useState<'operator' | 'supervisor'>('operator');

  // Sub-navigation items
  const subPages = [
    {
      id: 'operator-kiosk',
      name: 'Operator Kiosk',
      nameLocal: 'အော်ပရေတာကီယော့စ်',
      icon: UserCheck,
      description: 'Touchscreen UI for production logging',
      descriptionLocal: 'ထုတ်လုပ်မှုမှတ်တမ်းတင်ရန် ထိတွေ့မျက်နှာပြင်',
      color: 'bg-blue-600'
    },
    {
      id: 'glue-fill-operator',
      name: 'Glue Fill Operator',
      nameLocal: 'ကော်ဖြည့်အလုပ်သမား',
      icon: Droplets,
      description: 'RFID-based glue filling record system',
      descriptionLocal: 'RFID အခြေခံ ကော်ဖြည့်မှတ်တမ်းစနစ်',
      color: 'bg-teal-600'
    },
    {
      id: 'supervisor-screen',
      name: 'Supervisor Screen',
      nameLocal: 'ကြီးကြပ်သူမျက်နှာပြင်',
      icon: Monitor,
      description: 'Job completion & QR/RFID management',
      descriptionLocal: 'အလုပ်ပြီးစီးမှုနှင့် QR/RFID စီမံခန့်ခွဲမှု',
      color: 'bg-green-600'
    },
    {
      id: 'live-monitoring',
      name: 'Live Monitoring',
      nameLocal: 'တိုက်ရိုက်စောင့်ကြည့်ခြင်း',
      icon: Eye,
      description: 'Real-time machine status dashboard',
      descriptionLocal: 'စက်အခြေအနေ တိုက်ရိုက်ကြည့်ရှုမှု',
      color: 'bg-purple-600'
    },
    {
      id: 'operator-dashboard',
      name: 'Operator Dashboard',
      nameLocal: 'အော်ပရေတာဒက်ရှ်ဘုတ်',
      icon: Users,
      description: 'Real-time operator assignment management',
      descriptionLocal: 'အော်ပရေတာများ တိုက်ရိုက်ခန့်အပ်မှု စီမံခန့်ခွဲမှု',
      color: 'bg-orange-600'
    }
  ];

  const currentSubPage = subPages.find(page => page.id === currentPage);

  // If we're in a specific sub-page, render that component
  if (currentPage === 'operator-kiosk') {
    return <OperatorKiosk />;
  }

  if (currentPage === 'glue-fill-operator') {
    return (
      <GlueFillOperator 
        currentView={glueFillView} 
        onViewChange={setGlueFillView} 
      />
    );
  }
  
  if (currentPage === 'supervisor-screen') {
    return <SupervisorScreen />;
  }
  
  if (currentPage === 'live-monitoring') {
    return <LiveMonitoring />;
  }
  
  if (currentPage === 'operator-dashboard') {
    return <OperatorDashboard />;
  }

  // Main Production Module Dashboard
  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="mb-6">
          <div className="flex items-center gap-3 mb-4">
            <div className="p-2 bg-blue-100 rounded-lg">
              <Factory className="h-6 w-6 text-blue-600" />
            </div>
            <div>
              <h1 className="text-3xl font-semibold text-slate-900">
                Production Module | ထုတ်လုပ်မှုမော်ဂျူး
              </h1>
              <p className="text-slate-600 mt-1">
                Smart factory production management system
              </p>
              <p className="text-sm text-slate-500">
                စမတ်စက်မှုလုပ်ငန်း ထုတ်လုပ်မှုစီမံခန့်ခွဲမှုစနစ်
              </p>
            </div>
          </div>
        </div>

        {/* Quick Stats */}
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-green-100 rounded-lg">
                  <div className="w-3 h-3 bg-green-500 rounded-full"></div>
                </div>
                <div>
                  <div className="text-2xl font-bold text-green-700">8</div>
                  <div className="text-sm text-slate-600">Running Machines</div>
                  <div className="text-xs text-slate-500">လည်ပတ်နေသောစက်များ</div>
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
                  <div className="text-2xl font-bold text-yellow-700">2</div>
                  <div className="text-sm text-slate-600">Idle Machines</div>
                  <div className="text-xs text-slate-500">အလုပ်မရှိသောစက်များ</div>
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
                  <div className="text-2xl font-bold text-red-700">1</div>
                  <div className="text-sm text-slate-600">Down Machines</div>
                  <div className="text-xs text-slate-500">ပျက်နေသောစက်များ</div>
                </div>
              </div>
            </CardContent>
          </Card>

          <Card className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className="p-2 bg-blue-100 rounded-lg">
                  <UserCheck className="w-4 h-4 text-blue-600" />
                </div>
                <div>
                  <div className="text-2xl font-bold text-blue-700">24</div>
                  <div className="text-sm text-slate-600">Active Operators</div>
                  <div className="text-xs text-slate-500">အလုပ်လုပ်နေသောအော်ပရေတာများ</div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>

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

                  {/* Module-specific status/info */}
                  {page.id === 'operator-kiosk' && (
                    <div className="mb-4 space-y-2">
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Active Sessions:</span>
                        <Badge className="bg-green-100 text-green-800">12 Sessions</Badge>
                      </div>
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">RFID Connected:</span>
                        <Badge className="bg-blue-100 text-blue-800">8 Devices</Badge>
                      </div>
                    </div>
                  )}

                  {page.id === 'glue-fill-operator' && (
                    <div className="mb-4 space-y-2">
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Today's Records:</span>
                        <Badge className="bg-teal-100 text-teal-800">24 Fills</Badge>
                      </div>
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Active Operators:</span>
                        <Badge className="bg-blue-100 text-blue-800">6 Online</Badge>
                      </div>
                    </div>
                  )}

                  {page.id === 'supervisor-screen' && (
                    <div className="mb-4 space-y-2">
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Pending Approval:</span>
                        <Badge className="bg-yellow-100 text-yellow-800">3 Jobs</Badge>
                      </div>
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">QC Approved:</span>
                        <Badge className="bg-green-100 text-green-800">7 Jobs</Badge>
                      </div>
                    </div>
                  )}

                  {page.id === 'live-monitoring' && (
                    <div className="mb-4 space-y-2">
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Alerts Active:</span>
                        <Badge className="bg-red-100 text-red-800">2 Critical</Badge>
                      </div>
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Performance:</span>
                        <Badge className="bg-green-100 text-green-800">87% OEE</Badge>
                      </div>
                    </div>
                  )}

                  {page.id === 'operator-dashboard' && (
                    <div className="mb-4 space-y-2">
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Available Operators:</span>
                        <Badge className="bg-green-100 text-green-800">5 Free</Badge>
                      </div>
                      <div className="flex items-center justify-between text-sm">
                        <span className="text-slate-600">Assignments Today:</span>
                        <Badge className="bg-blue-100 text-blue-800">12 Changes</Badge>
                      </div>
                    </div>
                  )}

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

        {/* Recent Activity */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <div className="p-2 bg-slate-100 rounded-lg">
                <RefreshCw className="h-5 w-5 text-slate-600" />
              </div>
              <div>
                <div>Recent Production Activity</div>
                <div className="text-sm text-slate-500">လတ်တလောထုတ်လုပ်မှုလှုပ်ရှားမှု</div>
              </div>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-3">
              {[
                {
                  time: '15:32',
                  event: 'Machine INJ-M001 completed batch JOB20250907-001',
                  eventLocal: 'စက် INJ-M001 သည် batch JOB20250907-001 ပြီးစီးပြီ',
                  type: 'success'
                },
                {
                  time: '15:28',
                  event: 'Operator Ko Thant assigned to Machine INJ-M003',
                  eventLocal: 'အော်ပရေတာ ကိုသန့် အား စက် INJ-M003 သို့ ခန့်အပ်ပြီး',
                  type: 'info'
                },
                {
                  time: '15:15',
                  event: 'Machine INJ-M005 reported down - Maintenance required',
                  eventLocal: 'စက် INJ-M005 ပျက်နေကြောင်းအစီရင်ခံ - ပြုပြင်ထိန်းသိမ်းမှုလိုအပ်',
                  type: 'warning'
                },
                {
                  time: '15:10',
                  event: 'QC approved batch for Product 2011 - Plastic Bottle 500ml',
                  eventLocal: 'QC မှ ထုတ်ကုန် 2011 - ပလပ်စတစ်ပုလင်း 500မီလီ အတွက် batch အတည်ပြုပြီး',
                  type: 'success'
                },
                {
                  time: '15:05',
                  event: 'New production plan PLAN20250907-006 created',
                  eventLocal: 'ထုတ်လုပ်မှုအစီအစဉ်အသစ် PLAN20250907-006 ဖန်တီးပြီး',
                  type: 'info'
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