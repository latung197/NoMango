import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Badge } from '../ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Smartphone, Wifi, WifiOff, Monitor, Printer, QrCode } from 'lucide-react';

const mockDevices = [
  {
    id: 1,
    deviceId: 'DEV-01',
    name: 'RFID Kiosk Station A',
    type: 'RFID Kiosk',
    assignedMachine: 'MCH-01',
    assignedDept: 'Line A',
    status: 'Online',
    lastSeen: '2024-03-15 10:30'
  },
  {
    id: 2,
    deviceId: 'DEV-02',
    name: 'QR Printer Line B',
    type: 'QR Printer',
    assignedMachine: 'MCH-02',
    assignedDept: 'Line B',
    status: 'Online',
    lastSeen: '2024-03-15 10:25'
  },
  {
    id: 3,
    deviceId: 'DEV-03',
    name: 'Supervisor Tablet',
    type: 'Tablet',
    assignedMachine: null,
    assignedDept: 'Production',
    status: 'Offline',
    lastSeen: '2024-03-15 08:45'
  }
];

export function DeviceRegistration() {
  const [devices] = useState(mockDevices);

  const getDeviceIcon = (type: string) => {
    switch (type) {
      case 'RFID Kiosk':
        return <Monitor className="h-4 w-4" />;
      case 'QR Printer':
        return <Printer className="h-4 w-4" />;
      case 'Tablet':
        return <Smartphone className="h-4 w-4" />;
      case 'Scanner':
        return <QrCode className="h-4 w-4" />;
      default:
        return <Smartphone className="h-4 w-4" />;
    }
  };

  return (
    <div className="p-6 space-y-6">
      {/* Summary Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        {['Total Devices', 'Online', 'Offline', 'RFID Kiosks'].map((title, index) => (
          <Card key={title} className="border-0 shadow-sm">
            <CardContent className="p-4">
              <div className="flex items-center gap-3">
                <div className={`p-2 rounded-lg ${
                  index === 0 ? 'bg-blue-100' :
                  index === 1 ? 'bg-green-100' :
                  index === 2 ? 'bg-red-100' : 'bg-purple-100'
                }`}>
                  <Smartphone className={`w-4 h-4 ${
                    index === 0 ? 'text-blue-600' :
                    index === 1 ? 'text-green-600' :
                    index === 2 ? 'text-red-600' : 'text-purple-600'
                  }`} />
                </div>
                <div>
                  <div className={`text-2xl font-bold ${
                    index === 0 ? 'text-blue-700' :
                    index === 1 ? 'text-green-700' :
                    index === 2 ? 'text-red-700' : 'text-purple-700'
                  }`}>
                    {index === 0 ? devices.length :
                     index === 1 ? devices.filter(d => d.status === 'Online').length :
                     index === 2 ? devices.filter(d => d.status === 'Offline').length :
                     devices.filter(d => d.type === 'RFID Kiosk').length}
                  </div>
                  <div className="text-sm text-slate-600">{title}</div>
                  <div className="text-xs text-slate-500">
                    {index === 0 ? 'စုစုပေါင်းစက်ပစ္စည်းများ' :
                     index === 1 ? 'အွန်လိုင်းရှိနေသော' :
                     index === 2 ? 'အော့ဖ်လိုင်းဖြစ်နေသော' : 'RFID ကိုစကစ်များ'}
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {/* Main Content */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Smartphone className="h-5 w-5 text-blue-600" />
            <div>
              <div>Device Directory | စက်ပစ္စည်းလမ်းညွှန်</div>
              <div className="text-sm text-slate-500">Connected devices and equipment registry</div>
            </div>
          </CardTitle>
        </CardHeader>

        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Device ID | စက်ပစ္စည်းနံပါတ်</TableHead>
                  <TableHead>Name | အမည်</TableHead>
                  <TableHead>Type | အမျိုးအစား</TableHead>
                  <TableHead>Assigned Machine/Dept</TableHead>
                  <TableHead>Status | အခြေအနေ</TableHead>
                  <TableHead>Last Seen</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {devices.map((device) => (
                  <TableRow key={device.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono">
                        {device.deviceId}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-3">
                        <div className="p-2 bg-slate-100 rounded-lg">
                          {getDeviceIcon(device.type)}
                        </div>
                        <div className="font-medium">{device.name}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline">{device.type}</Badge>
                    </TableCell>
                    <TableCell>
                      <div>
                        {device.assignedMachine && (
                          <div className="font-medium">{device.assignedMachine}</div>
                        )}
                        <div className="text-sm text-slate-500">{device.assignedDept}</div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center gap-2">
                        {device.status === 'Online' ? (
                          <Wifi className="h-4 w-4 text-green-600" />
                        ) : (
                          <WifiOff className="h-4 w-4 text-red-600" />
                        )}
                        <Badge className={
                          device.status === 'Online' 
                            ? 'bg-green-100 text-green-800' 
                            : 'bg-red-100 text-red-800'
                        }>
                          {device.status}
                        </Badge>
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm">{device.lastSeen}</div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm text-slate-500">Configure</div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}