import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Wrench, Search, Filter, Download, Plus, Edit } from 'lucide-react';

// Mock data for molds
const mockMolds = [
  {
    id: 1,
    moldCode: 'MOLD-201',
    name: 'Injection Mold A',
    linkedProducts: ['Product A', 'Product B'],
    compatibleMachines: ['MCH-01', 'MCH-02'],
    cavities: 4,
    runnerWeight: 12,
    lifetimeShots: 200000,
    currentShots: 45000,
    status: 'Active'
  },
  {
    id: 2,
    moldCode: 'MOLD-202',
    name: 'Cutting Tool B',
    linkedProducts: ['Product C'],
    compatibleMachines: ['MCH-03'],
    cavities: 1,
    runnerWeight: 0,
    lifetimeShots: 500000,
    currentShots: 125000,
    status: 'Active'
  }
];

export function MoldToolingRegistration() {
  const [molds] = useState(mockMolds);

  return (
    <div className="p-6 space-y-6">
      {/* Summary Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-orange-100 rounded-lg">
                <Wrench className="w-4 h-4 text-orange-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-orange-700">{molds.length}</div>
                <div className="text-sm text-slate-600">Total Molds</div>
                <div className="text-xs text-slate-500">စုစုပေါင်းပုံစံများ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-green-100 rounded-lg">
                <Wrench className="w-4 h-4 text-green-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-green-700">
                  {molds.filter(m => m.status === 'Active').length}
                </div>
                <div className="text-sm text-slate-600">Active Molds</div>
                <div className="text-xs text-slate-500">အသုံးပြုနေသောပုံစံများ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-blue-100 rounded-lg">
                <Wrench className="w-4 h-4 text-blue-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-blue-700">
                  {molds.reduce((sum, m) => sum + m.cavities, 0)}
                </div>
                <div className="text-sm text-slate-600">Total Cavities</div>
                <div className="text-xs text-slate-500">စုစုပေါင်းအပေါက်များ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-purple-100 rounded-lg">
                <Wrench className="w-4 h-4 text-purple-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-purple-700">
                  {Math.round(molds.reduce((sum, m) => sum + (m.currentShots / m.lifetimeShots * 100), 0) / molds.length)}%
                </div>
                <div className="text-sm text-slate-600">Avg Usage</div>
                <div className="text-xs text-slate-500">ပျမ်းမျှအသုံးပြုမှု</div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Main Content */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <Wrench className="h-5 w-5 text-orange-600" />
              <div>
                <div>Mold & Tooling Directory | ပုံစံနှင့်ကိရိယာလမ်းညွှန်</div>
                <div className="text-sm text-slate-500">Production molds and tooling equipment</div>
              </div>
            </CardTitle>
            <div className="flex items-center gap-3">
              <Button variant="outline" className="gap-2">
                <Download className="h-4 w-4" />
                Export
              </Button>
              <Button className="gap-2 bg-orange-600 hover:bg-orange-700">
                <Plus className="h-4 w-4" />
                Add Mold | ပုံစံအသစ်ထည့်ရန်
              </Button>
            </div>
          </div>
        </CardHeader>

        <CardContent>
          {/* Molds Table */}
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Mold Code | ပုံစံကုဒ်</TableHead>
                  <TableHead>Name | အမည်</TableHead>
                  <TableHead>Linked Product | ဆက်စပ်ထုတ်ကုန်</TableHead>
                  <TableHead>Compatible Machines</TableHead>
                  <TableHead>Cavities | အပေါက်များ</TableHead>
                  <TableHead>Runner Weight | ပြေးလမ်းအလေးချိန်</TableHead>
                  <TableHead>Lifetime Usage</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {molds.map((mold) => (
                  <TableRow key={mold.id}>
                    <TableCell>
                      <Badge variant="outline" className="font-mono">
                        {mold.moldCode}
                      </Badge>
                    </TableCell>
                    <TableCell>
                      <div className="font-medium">{mold.name}</div>
                    </TableCell>
                    <TableCell>
                      <div className="space-y-1">
                        {mold.linkedProducts.map((product, index) => (
                          <Badge key={index} variant="outline" className="text-xs mr-1">
                            {product}
                          </Badge>
                        ))}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="space-y-1">
                        {mold.compatibleMachines.map((machine, index) => (
                          <Badge key={index} variant="outline" className="text-xs mr-1 bg-blue-50">
                            {machine}
                          </Badge>
                        ))}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="font-medium text-lg">{mold.cavities}</div>
                    </TableCell>
                    <TableCell>
                      <div className="font-medium">{mold.runnerWeight}g</div>
                    </TableCell>
                    <TableCell>
                      <div>
                        <div className="font-medium">
                          {mold.currentShots.toLocaleString()} / {mold.lifetimeShots.toLocaleString()}
                        </div>
                        <div className="text-xs text-slate-500">
                          {Math.round(mold.currentShots / mold.lifetimeShots * 100)}% used
                        </div>
                        <div className="w-full bg-slate-200 rounded-full h-1 mt-1">
                          <div 
                            className="bg-orange-500 h-1 rounded-full" 
                            style={{ width: `${mold.currentShots / mold.lifetimeShots * 100}%` }}
                          ></div>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Button
                        variant="outline"
                        size="sm"
                        className="gap-1"
                      >
                        <Edit className="h-3 w-3" />
                        Edit
                      </Button>
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