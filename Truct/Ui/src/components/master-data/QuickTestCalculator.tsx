import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Badge } from '../ui/badge';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { 
  Calculator,
  Play,
  Download,
  ExternalLink,
  Clock,
  Zap,
  Package,
  TrendingUp,
  Info,
  Link,
  Archive
} from 'lucide-react';

// Mock data
const mockProductColors = [
  { 
    code: 'CUP-250-RED-001', 
    product: 'CUP-250', 
    color: 'RED-001',
    allowedMolds: ['MLD-001', 'MLD-002'],
    allowedMachines: ['MCH-001', 'MCH-002']
  },
  { 
    code: 'CUP-500-BLUE-002', 
    product: 'CUP-500', 
    color: 'BLUE-002',
    allowedMolds: ['MLD-004'],
    allowedMachines: ['MCH-002', 'MCH-003']
  }
];

const mockMolds = [
  { code: 'MLD-001', name: 'Cup Mold 4-Cavity', cavities: 4 },
  { code: 'MLD-002', name: 'Cup Mold 8-Cavity', cavities: 8 },
  { code: 'MLD-004', name: 'Large Cup Mold 2-Cavity', cavities: 2 }
];

const mockMachines = [
  { code: 'MCH-001', name: 'Injection Machine 1' },
  { code: 'MCH-002', name: 'Injection Machine 2' },
  { code: 'MCH-003', name: 'Injection Machine 3' }
];

// Mock data lookups
const mockWeightData = {
  'CUP-250-RED-001': { partWeight: 15.5, runnerWeight: 12.2 },
  'CUP-500-BLUE-002': { partWeight: 28.0, runnerWeight: 18.5 }
};

const mockCTData = {
  'CUP-250-RED-001-MLD-001-MCH-001': { ct: 28, efficiency: 85 },
  'CUP-250-RED-001-MCH-002': { ct: 32, efficiency: 80 },
  'CUP-250-RED-001': { ct: 30, efficiency: 82 },
  'CUP-500-BLUE-002-MLD-004-MCH-002': { ct: 45, efficiency: 88 }
};

const mockRecipeData = {
  'CUP-250-RED-001': [
    { materialCode: 'PP-RED-001', materialName: 'Red Polypropylene', percent: 85 },
    { materialCode: 'ADDITIVE-001', materialName: 'UV Stabilizer', percent: 15 }
  ]
};

export function QuickTestCalculator() {
  const [selectedProductColor, setSelectedProductColor] = useState('');
  const [selectedMold, setSelectedMold] = useState('');
  const [selectedMachine, setSelectedMachine] = useState('');
  const [orderQuantity, setOrderQuantity] = useState('1000');
  const [testResults, setTestResults] = useState<any>(null);

  // Auto-populate data based on selections
  const getAvailableMolds = () => {
    if (!selectedProductColor) return [];
    const productColor = mockProductColors.find(pc => pc.code === selectedProductColor);
    return mockMolds.filter(mold => productColor?.allowedMolds.includes(mold.code) || false);
  };

  const getAvailableMachines = () => {
    if (!selectedProductColor) return [];
    const productColor = mockProductColors.find(pc => pc.code === selectedProductColor);
    return mockMachines.filter(machine => productColor?.allowedMachines.includes(machine.code) || false);
  };

  const getCTAndEfficiency = () => {
    if (!selectedProductColor) return { ct: 0, efficiency: 85 };
    
    // Priority matching: Color+Mold+Machine > Color+Machine > Color default
    const keys = [
      `${selectedProductColor}-${selectedMold}-${selectedMachine}`,
      `${selectedProductColor}-${selectedMachine}`,
      selectedProductColor
    ];
    
    for (const key of keys) {
      if (mockCTData[key]) {
        return mockCTData[key];
      }
    }
    
    return { ct: 30, efficiency: 85 };
  };

  const getWeightData = () => {
    return mockWeightData[selectedProductColor] || { partWeight: 0, runnerWeight: 0 };
  };

  const getCavities = () => {
    if (!selectedMold) return 1;
    const mold = mockMolds.find(m => m.code === selectedMold);
    return mold?.cavities || 1;
  };

  const getBagSizeKg = () => {
    // From constants: 55 lb * 454.54 g/lb / 1000 = 25.0 kg
    return 25.0;
  };

  const runTest = () => {
    const qty = parseInt(orderQuantity) || 0;
    const weightData = getWeightData();
    const { ct, efficiency } = getCTAndEfficiency();
    const cavities = getCavities();
    const bagSize = getBagSizeKg();
    
    if (!selectedProductColor || !selectedMold || !selectedMachine || qty === 0) {
      return;
    }

    // Calculate weight per shot
    const weightPerShot = (weightData.partWeight + weightData.runnerWeight) * cavities / 1000; // kg
    
    // Calculate shots needed
    const shotsNeeded = Math.ceil(qty / cavities);
    
    // Calculate theoretical time (no efficiency applied)
    const theoreticalTimeHours = (shotsNeeded * ct) / 3600;
    
    // Calculate planned time (with efficiency)
    const plannedTimeHours = theoreticalTimeHours / (efficiency / 100);
    
    // Calculate theoretical and planned units per hour
    const theoreticalUnitsPerHour = (3600 / ct) * cavities;
    const plannedUnitsPerHour = theoreticalUnitsPerHour * (efficiency / 100);
    
    // Calculate material required
    const materialKg = (shotsNeeded * weightPerShot) + 2.5; // +2.5kg purge
    const bagsNeeded = Math.ceil(materialKg / bagSize);
    
    // Calculate per-material breakdown if recipe exists
    let materialBreakdown = null;
    const recipe = mockRecipeData[selectedProductColor];
    if (recipe) {
      materialBreakdown = recipe.map(item => ({
        ...item,
        kg: (materialKg * item.percent / 100),
        bags: Math.ceil((materialKg * item.percent / 100) / bagSize)
      }));
    }

    const result = {
      unitsPerHour: {
        theoretical: Math.round(theoreticalUnitsPerHour),
        planned: Math.round(plannedUnitsPerHour)
      },
      duration: {
        theoretical: theoreticalTimeHours.toFixed(2),
        planned: plannedTimeHours.toFixed(2)
      },
      material: {
        totalKg: materialKg.toFixed(2),
        totalBags: bagsNeeded,
        breakdown: materialBreakdown
      },
      assumptions: {
        productColor: selectedProductColor,
        mold: selectedMold,
        machine: selectedMachine,
        cavities,
        partWeight: weightData.partWeight,
        runnerWeight: weightData.runnerWeight,
        weightPerShot: (weightPerShot * 1000).toFixed(2), // back to grams for display
        cycleTime: ct,
        efficiency,
        bagSizeKg: bagSize,
        purgeKg: 2.5
      }
    };

    setTestResults(result);
  };

  const clearTest = () => {
    setSelectedProductColor('');
    setSelectedMold('');
    setSelectedMachine('');
    setOrderQuantity('1000');
    setTestResults(null);
  };

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">
          Quick Test Calculator | မြန်ဆန်သောစမ်းသပ်တွက်စက်
        </h2>
        <p className="text-slate-600 mt-1">
          Quickly calculate production requirements for any product-color-mold-machine combination
        </p>
        <p className="text-sm text-slate-500">
          မည်သည့်ထုတ်ကုန်-အရောင်-ပုံစံ-စက်ပေါင်းစပ်မှုအတွက်မဆို ထုတ်လုပ်မှုလိုအပ်ချက်များကို မြန်ဆန်စွာတွက်ချက်ခြင်း
        </p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Input Panel */}
        <div className="space-y-6">
          {/* Step 1: Select Product Color */}
          <Card className="border-0 shadow-sm">
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Package className="h-5 w-5 text-blue-600" />
                Step 1: Select Product Color | ခြေလှမ်း ၁: ထုတ်ကုန်အရောင်ရွေးချယ်ပါ
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Label>Product Color Code | ထုတ်ကုန်အရောင်ကုဒ်</Label>
              <Select value={selectedProductColor} onValueChange={(value) => {
                setSelectedProductColor(value);
                setSelectedMold('');
                setSelectedMachine('');
                setTestResults(null);
              }}>
                <SelectTrigger className="mt-1">
                  <SelectValue placeholder="Select product color..." />
                </SelectTrigger>
                <SelectContent>
                  {mockProductColors.map((pc) => (
                    <SelectItem key={pc.code} value={pc.code}>
                      <div className="flex items-center gap-2">
                        <Badge variant="outline" className="text-xs">{pc.product}</Badge>
                        <span>-</span>
                        <Badge variant="outline" className="text-xs">{pc.color}</Badge>
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </CardContent>
          </Card>

          {/* Step 2: Auto-populated dropdowns */}
          {selectedProductColor && (
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Zap className="h-5 w-5 text-green-600" />
                  Step 2: Select Configuration | ခြေလှမ်း ၂: ပြင်ဆင်မှုရွေးချယ်ပါ
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <Label>Mold | ပုံစံ</Label>
                    <Select value={selectedMold} onValueChange={(value) => {
                      setSelectedMold(value);
                      setTestResults(null);
                    }}>
                      <SelectTrigger className="mt-1">
                        <SelectValue placeholder="Select mold..." />
                      </SelectTrigger>
                      <SelectContent>
                        {getAvailableMolds().map((mold) => (
                          <SelectItem key={mold.code} value={mold.code}>
                            {mold.code} - {mold.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>

                  <div>
                    <Label>Machine | စက်</Label>
                    <Select value={selectedMachine} onValueChange={(value) => {
                      setSelectedMachine(value);
                      setTestResults(null);
                    }}>
                      <SelectTrigger className="mt-1">
                        <SelectValue placeholder="Select machine..." />
                      </SelectTrigger>
                      <SelectContent>
                        {getAvailableMachines().map((machine) => (
                          <SelectItem key={machine.code} value={machine.code}>
                            {machine.code} - {machine.name}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                </div>

                {selectedMold && selectedMachine && (
                  <div className="p-3 bg-green-50 border border-green-200 rounded-lg">
                    <div className="text-sm">
                      <strong>Auto-populated values:</strong><br />
                      CT: {getCTAndEfficiency().ct} sec | Efficiency: {getCTAndEfficiency().efficiency}% | Cavities: {getCavities()} | Part Weight: {getWeightData().partWeight}g | Runner: {getWeightData().runnerWeight}g | Bag Size: {getBagSizeKg()}kg
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          )}

          {/* Step 3: Order Quantity */}
          {selectedMold && selectedMachine && (
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Calculator className="h-5 w-5 text-purple-600" />
                  Step 3: Order Quantity | ခြေလှမ်း ၃: မှာယူအရေအတွက်
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <Label htmlFor="orderQuantity">Order Quantity (pcs) | မှာယူအရေအတွက်</Label>
                  <Input
                    id="orderQuantity"
                    type="number"
                    value={orderQuantity}
                    onChange={(e) => {
                      setOrderQuantity(e.target.value);
                      setTestResults(null);
                    }}
                    placeholder="1000"
                    className="mt-1"
                  />
                </div>

                <div className="flex gap-3">
                  <Button onClick={runTest} className="flex-1 gap-2 bg-green-600 hover:bg-green-700">
                    <Play className="h-4 w-4" />
                    Run Test | စမ်းသပ်မှုလုပ်ဆောင်ရန်
                  </Button>
                  <Button variant="outline" onClick={clearTest}>
                    Clear | ရှင်းလင်းရန်
                  </Button>
                </div>
              </CardContent>
            </Card>
          )}
        </div>

        {/* Results Panel */}
        <div className="space-y-6">
          {testResults ? (
            <>
              {/* Production Rate Results */}
              <Card className="border-0 shadow-sm">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <TrendingUp className="h-5 w-5 text-blue-600" />
                    Production Rate | ထုတ်လုပ်မှုနှုန်း
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 gap-4">
                    <div className="text-center p-4 bg-blue-50 rounded-lg">
                      <div className="text-2xl font-bold text-blue-700">
                        {testResults.unitsPerHour.theoretical}
                      </div>
                      <div className="text-sm text-blue-600">Units/hour (Theoretical)</div>
                      <div className="text-xs text-slate-500">စိတ်ကူးပိုင်းဆိုင်ရာ</div>
                    </div>
                    <div className="text-center p-4 bg-green-50 rounded-lg">
                      <div className="text-2xl font-bold text-green-700">
                        {testResults.unitsPerHour.planned}
                      </div>
                      <div className="text-sm text-green-600">Units/hour (Planned)</div>
                      <div className="text-xs text-slate-500">စီမံချက်</div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Duration Results */}
              <Card className="border-0 shadow-sm">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Clock className="h-5 w-5 text-orange-600" />
                    Duration | ကြာချိန်
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 gap-4">
                    <div className="text-center p-4 bg-orange-50 rounded-lg">
                      <div className="text-2xl font-bold text-orange-700">
                        {testResults.duration.theoretical}
                      </div>
                      <div className="text-sm text-orange-600">Hours (Theoretical)</div>
                      <div className="text-xs text-slate-500">နာရီ (စိတ်ကူးပိုင်းဆိုင်ရာ)</div>
                    </div>
                    <div className="text-center p-4 bg-red-50 rounded-lg">
                      <div className="text-2xl font-bold text-red-700">
                        {testResults.duration.planned}
                      </div>
                      <div className="text-sm text-red-600">Hours (Planned)</div>
                      <div className="text-xs text-slate-500">နာရီ (စီမံချက်)</div>
                    </div>
                  </div>
                </CardContent>
              </Card>

              {/* Material Results */}
              <Card className="border-0 shadow-sm">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Package className="h-5 w-5 text-green-600" />
                    Material Required | လိုအပ်သောပစ္စည်း
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 gap-4 mb-4">
                    <div className="text-center p-4 bg-green-50 rounded-lg">
                      <div className="text-2xl font-bold text-green-700">
                        {testResults.material.totalKg}
                      </div>
                      <div className="text-sm text-green-600">Total (kg)</div>
                      <div className="text-xs text-slate-500">စုစုပေါင်း</div>
                    </div>
                    <div className="text-center p-4 bg-purple-50 rounded-lg">
                      <div className="text-2xl font-bold text-purple-700">
                        {testResults.material.totalBags}
                      </div>
                      <div className="text-sm text-purple-600">Bags Needed</div>
                      <div className="text-xs text-slate-500">လိုအပ်သောအိတ်များ</div>
                    </div>
                  </div>

                  {/* Recipe Breakdown */}
                  {testResults.material.breakdown && (
                    <div>
                      <h4 className="font-medium mb-2">Material Breakdown | ပစ္စည်းခွဲခြမ်းစိတ်ဖြာမှု:</h4>
                      <Table>
                        <TableHeader>
                          <TableRow>
                            <TableHead>Material | ပစ္စည်း</TableHead>
                            <TableHead>%</TableHead>
                            <TableHead>kg</TableHead>
                            <TableHead>Bags</TableHead>
                          </TableRow>
                        </TableHeader>
                        <TableBody>
                          {testResults.material.breakdown.map((item: any, index: number) => (
                            <TableRow key={index}>
                              <TableCell>
                                <div>
                                  <div className="font-medium">{item.materialCode}</div>
                                  <div className="text-xs text-slate-500">{item.materialName}</div>
                                </div>
                              </TableCell>
                              <TableCell>{item.percent}%</TableCell>
                              <TableCell>{item.kg.toFixed(2)}</TableCell>
                              <TableCell>{item.bags}</TableCell>
                            </TableRow>
                          ))}
                        </TableBody>
                      </Table>
                    </div>
                  )}
                </CardContent>
              </Card>

              {/* Assumptions Used */}
              <Card className="border-0 shadow-sm">
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <Info className="h-5 w-5 text-slate-600" />
                    Assumptions Used | အသုံးပြုသောယူဆချက်များ
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 gap-2 text-sm">
                    <div className="flex justify-between">
                      <span>Product Color:</span>
                      <Badge variant="outline" className="text-xs">
                        {testResults.assumptions.productColor}
                      </Badge>
                    </div>
                    <div className="flex justify-between">
                      <span>Mold:</span>
                      <Badge variant="outline" className="text-xs">
                        {testResults.assumptions.mold}
                      </Badge>
                    </div>
                    <div className="flex justify-between">
                      <span>Machine:</span>
                      <Badge variant="outline" className="text-xs">
                        {testResults.assumptions.machine}
                      </Badge>
                    </div>
                    <div className="flex justify-between">
                      <span>Cavities:</span>
                      <span>{testResults.assumptions.cavities}</span>
                    </div>
                    <div className="flex justify-between">
                      <span>Part Weight:</span>
                      <span>{testResults.assumptions.partWeight}g</span>
                    </div>
                    <div className="flex justify-between">
                      <span>Runner Weight:</span>
                      <span>{testResults.assumptions.runnerWeight}g</span>
                    </div>
                    <div className="flex justify-between">
                      <span>Weight/Shot:</span>
                      <span>{testResults.assumptions.weightPerShot}g</span>
                    </div>
                    <div className="flex justify-between">
                      <span>Cycle Time:</span>
                      <span>{testResults.assumptions.cycleTime}s</span>
                    </div>
                    <div className="flex justify-between">
                      <span>Efficiency:</span>
                      <span>{testResults.assumptions.efficiency}%</span>
                    </div>
                    <div className="flex justify-between">
                      <span>Bag Size:</span>
                      <span>{testResults.assumptions.bagSizeKg}kg</span>
                    </div>
                    <div className="flex justify-between">
                      <span>Purge Loss:</span>
                      <span>{testResults.assumptions.purgeKg}kg</span>
                    </div>
                  </div>

                  <div className="mt-4 text-xs text-slate-500">
                    <Link className="inline h-3 w-3 mr-1" />
                    Values automatically pulled from Weight Register, CT Register, and Constants
                  </div>
                </CardContent>
              </Card>

              {/* Action Buttons */}
              <Card className="border-0 shadow-sm">
                <CardContent className="p-4">
                  <div className="flex gap-3">
                    <Button className="flex-1 gap-2" variant="outline">
                      <ExternalLink className="h-4 w-4" />
                      Use in Planning | စီမံကိန်းတွင်အသုံးပြုရန်
                    </Button>
                    <Button className="flex-1 gap-2" variant="outline">
                      <Download className="h-4 w-4" />
                      Export CSV
                    </Button>
                  </div>
                </CardContent>
              </Card>
            </>
          ) : (
            <Card className="border-0 shadow-sm">
              <CardContent className="p-12 text-center">
                <Calculator className="h-16 w-16 text-slate-300 mx-auto mb-4" />
                <h3 className="font-medium text-slate-600 mb-2">Ready to Calculate</h3>
                <p className="text-sm text-slate-500">
                  Select product color, mold, machine, and enter order quantity to run test calculation
                </p>
                <p className="text-xs text-slate-400 mt-1">
                  ထုတ်ကုန်အရောင်၊ ပုံစံ၊ စက်နှင့် မှာယူအရေအတွက်ကို ရွေးချယ်ပြီး စမ်းသပ်တွက်ချက်မှုလုပ်ဆောင်ပါ
                </p>
              </CardContent>
            </Card>
          )}
        </div>
      </div>
    </div>
  );
}