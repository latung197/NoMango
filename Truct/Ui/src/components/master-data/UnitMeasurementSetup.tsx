import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Badge } from '../ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Button } from '../ui/button';
import { Scale, ArrowRightLeft, Calculator } from 'lucide-react';

const mockUOMs = [
  {
    id: 1,
    uomCode: 'KG',
    baseUnit: 'kg',
    description: 'Kilogram - Base weight unit',
    descriptionMM: 'ကီလိုဂရမ် - အခြေခံအလေးချိန်ယူနစ်',
    conversions: [
      { from: 'g', to: 'kg', factor: 0.001 },
      { from: 'lb', to: 'kg', factor: 0.453592 },
      { from: 'ton', to: 'kg', factor: 1000 }
    ],
    status: 'Active'
  },
  {
    id: 2,
    uomCode: 'PCS',
    baseUnit: 'pcs',
    description: 'Pieces - Count unit',
    descriptionMM: 'အရေအတွက် - ရေတွက်ယူနစ်',
    conversions: [
      { from: 'dozen', to: 'pcs', factor: 12 },
      { from: 'gross', to: 'pcs', factor: 144 }
    ],
    status: 'Active'
  },
  {
    id: 3,
    uomCode: 'M',
    baseUnit: 'm',
    description: 'Meter - Length unit',
    descriptionMM: 'မီတာ - အလျားယူနစ်',
    conversions: [
      { from: 'cm', to: 'm', factor: 0.01 },
      { from: 'mm', to: 'm', factor: 0.001 },
      { from: 'ft', to: 'm', factor: 0.3048 }
    ],
    status: 'Active'
  }
];

export function UnitMeasurementSetup() {
  const [uoms] = useState(mockUOMs);

  return (
    <div className="p-6 space-y-6">
      {/* Summary Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-blue-100 rounded-lg">
                <Scale className="w-4 h-4 text-blue-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-blue-700">{uoms.length}</div>
                <div className="text-sm text-slate-600">Base Units</div>
                <div className="text-xs text-slate-500">အခြေခံယူနစ်များ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-green-100 rounded-lg">
                <ArrowRightLeft className="w-4 h-4 text-green-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-green-700">
                  {uoms.reduce((sum, uom) => sum + uom.conversions.length, 0)}
                </div>
                <div className="text-sm text-slate-600">Conversions</div>
                <div className="text-xs text-slate-500">ပြောင်းလဲမှုများ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-purple-100 rounded-lg">
                <Calculator className="w-4 h-4 text-purple-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-purple-700">Auto</div>
                <div className="text-sm text-slate-600">Conversion</div>
                <div className="text-xs text-slate-500">အလိုအလျောက်ပြောင်းလဲမှု</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-orange-100 rounded-lg">
                <Scale className="w-4 h-4 text-orange-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-orange-700">
                  {uoms.filter(u => u.status === 'Active').length}
                </div>
                <div className="text-sm text-slate-600">Active</div>
                <div className="text-xs text-slate-500">အသုံးပြုနေသော</div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* UOM Behavior Info */}
      <Card className="border-0 shadow-sm bg-blue-50 border-blue-200">
        <CardContent className="p-4">
          <div className="flex items-start gap-3">
            <div className="p-2 bg-blue-100 rounded-lg">
              <Calculator className="h-5 w-5 text-blue-600" />
            </div>
            <div>
              <h3 className="font-semibold text-blue-900 mb-2">
                Auto-Conversion Behavior | အလိုအလျောက်ပြောင်းလဲမှုအမူအကျင့်
              </h3>
              <div className="space-y-2 text-sm">
                <p className="text-blue-800">
                  • Operators can enter values in any supported unit (lb, g, ton, etc.)
                </p>
                <p className="text-blue-700">
                  • System automatically converts to base unit (kg) for all calculations
                </p>
                <p className="text-blue-700">  
                  • Reports & dashboards always display base units
                </p>
                <p className="text-blue-700">
                  • Toggle available for customer-preferred unit export
                </p>
                <p className="text-sm text-blue-600 mt-2">
                  အလုပ်သမားများသည် မည်သည့်ယူနစ်ဖြင့်မဆို ရိုက်ထည့်နိုင်ပြီး စနစ်က အလိုအလျောက် အခြေခံယူနစ်သို့ ပြောင်းလဲပေးသည်။
                </p>
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Main Content */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Scale className="h-5 w-5 text-blue-600" />
            <div>
              <div>Unit of Measurement Setup | တိုင်းတာမှုယူနစ်စနစ်</div>
              <div className="text-sm text-slate-500">Configure measurement units and conversion rules</div>
            </div>
          </CardTitle>
        </CardHeader>

        <CardContent>
          <div className="space-y-6">
            {uoms.map((uom) => (
              <Card key={uom.id} className="border border-slate-200">
                <CardContent className="p-4">
                  <div className="flex items-center justify-between mb-4">
                    <div className="flex items-center gap-3">
                      <Badge variant="outline" className="font-mono text-lg px-3 py-1">
                        {uom.uomCode}
                      </Badge>
                      <div>
                        <div className="font-semibold">{uom.description}</div>
                        <div className="text-sm text-slate-500">{uom.descriptionMM}</div>
                      </div>
                    </div>
                    <div className="flex items-center gap-2">
                      <Badge className="bg-blue-100 text-blue-800">
                        Base Unit: {uom.baseUnit}
                      </Badge>
                      <Badge className={
                        uom.status === 'Active' 
                          ? 'bg-green-100 text-green-800' 
                          : 'bg-red-100 text-red-800'
                      }>
                        {uom.status}
                      </Badge>
                    </div>
                  </div>

                  <div>
                    <h4 className="font-medium mb-3 flex items-center gap-2">
                      <ArrowRightLeft className="h-4 w-4" />
                      Conversion Rules | ပြောင်းလဲမှုစည်းမျဉ်းများ
                    </h4>
                    <div className="overflow-x-auto">
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>From Unit</TableHead>
                            <TableHead>To Unit</TableHead>
                            <TableHead>Conversion Factor</TableHead>
                            <TableHead>Example</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {uom.conversions.map((conversion, index) => (
                            <TableRow key={index}>
                              <TableCell>
                                <Badge variant="outline">{conversion.from}</Badge>
                              </TableCell>
                              <TableCell>
                                <Badge variant="outline" className="bg-blue-50">
                                  {conversion.to}
                                </Badge>
                              </TableCell>
                              <TableCell>
                                <code className="bg-slate-100 px-2 py-1 rounded text-sm">
                                  × {conversion.factor}
                                </code>
                              </TableCell>
                              <TableCell className="text-sm text-slate-600">
                                1 {conversion.from} = {conversion.factor} {conversion.to}
                              </TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>

          <div className="mt-6 pt-4 border-t border-slate-200">
            <Button className="gap-2 bg-blue-600 hover:bg-blue-700">
              <Scale className="h-4 w-4" />
              Add Unit Setup | ယူနစ်စနစ်အသစ်ထည့်ရန်
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}