import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '../ui/select';
import { Textarea } from '../ui/textarea';
import { Switch } from '../ui/switch';
import { 
  Calculator, 
  Play, 
  ChevronRight,
  Settings,
  Package,
  Clock,
  Zap,
  Info,
  Plus,
  Edit
} from 'lucide-react';

// Predefined variables with friendly names and icons
const predefinedVariables = [
  {
    id: 'orderQty',
    name: 'Order Quantity',
    nameMM: 'မှာယူသောအရေအတွက်',
    icon: '📦',
    unit: 'pcs',
    defaultValue: 1000,
    notes: 'Total pieces to produce',
    editable: true
  },
  {
    id: 'cavities',
    name: 'Cavities',
    nameMM: 'အပေါက်များ',
    icon: '🔧',
    unit: 'count',
    defaultValue: 4,
    notes: 'Number of cavities in mold',
    editable: false
  },
  {
    id: 'partWeight',
    name: 'Part Weight',
    nameMM: 'အစိတ်အပိုင်းအလေးချိန်',
    icon: '⚖️',
    unit: 'g',
    defaultValue: 15,
    notes: 'Weight of single part',
    editable: false
  },
  {
    id: 'runnerWeight',
    name: 'Runner Weight',
    nameMM: 'ပြေးလမ်းအလေးချိន်',
    icon: '🔗',
    unit: 'g',
    defaultValue: 12,
    notes: 'Weight of runner system',
    editable: false
  },
  {
    id: 'cycleTime',
    name: 'Cycle Time',
    nameMM: 'ပတ်လည်ချိန်',
    icon: '⏱️',
    unit: 'sec',
    defaultValue: 30,
    notes: 'Time per production cycle',
    editable: false
  },
  {
    id: 'efficiency',
    name: 'Efficiency',
    nameMM: 'စွမ်းရည်',
    icon: '⚡',
    unit: '%',
    defaultValue: 85,
    notes: 'Production efficiency rate',
    editable: true
  },
  {
    id: 'rejectRate',
    name: 'Reject Rate',
    nameMM: 'ပယ်ချနှုန်း',
    icon: '❌',
    unit: '%',
    defaultValue: 2,
    notes: 'Defect rejection percentage',
    editable: true
  },
  {
    id: 'purgeLoss',
    name: 'Purge Loss',
    nameMM: 'သန့်စင်ဆုံးရှုံးမှု',
    icon: '🧹',
    unit: 'kg',
    defaultValue: 2.5,
    notes: 'Material lost during purging',
    editable: true
  },
  {
    id: 'bagSize',
    name: 'Bag Size',
    nameMM: 'အိတ်အရွယ်အစား',
    icon: '🛍️',
    unit: 'kg',
    defaultValue: 25,
    notes: 'Standard material bag weight',
    editable: true
  }
];

// Formula templates with plain language descriptions
const formulaTemplates = [
  {
    id: 'material-requirement',
    name: 'Material Requirement',
    nameMM: 'ပစ္စည်းလိုအပ်ချက်',
    icon: '📦',
    color: 'bg-blue-50 border-blue-200',
    description: 'Calculate total material needed in kg',
    descriptionMM: 'လိုအပ်သောပစ္စည်းစုစုပေါင်းကို ကီလိုဂရမ်ဖြင့်တွက်ချက်',
    formula: 'Material (kg) = (Order Quantity × Part Weight ÷ Efficiency) + Purge Loss',
    formulaMM: 'ပစ္စည်း (ကီလို) = (မှာယူအရေအတွက် × အစိတ်အလေးချိန် ÷ စွမ်းရည်) + သန့်စင်ဆုံးရှုံးမှု',
    outputUnit: 'kg',
    selected: true
  },
  {
    id: 'bags-required',
    name: 'Bags Required',
    nameMM: 'လိုအပ်သောအိတ်များ',
    icon: '🛍️',
    color: 'bg-green-50 border-green-200',
    description: 'Calculate number of material bags needed',
    descriptionMM: 'လိုအပ်သောပစ္စည်းအိတ်အရေအတွက်တွက်ချက်',
    formula: 'Bags = Material Required (kg) ÷ Bag Size (kg)',
    formulaMM: 'အိတ်များ = လိုအပ်သောပစ္စည်း (ကီလို) ÷ အိတ်အရွယ် (ကီလို)',
    outputUnit: 'bags',
    selected: false
  },
  {
    id: 'production-duration',
    name: 'Production Duration',
    nameMM: 'ထုတ်လုပ်မှုကြာချိန်',
    icon: '⏱️',
    color: 'bg-orange-50 border-orange-200',
    description: 'Calculate total production time in hours',
    descriptionMM: 'စုစုပေါင်းထုတ်လုပ်မှုအချိန်ကို နာရီဖြင့်တွက်ချက်',
    formula: 'Duration (hrs) = (Order Quantity ÷ Cavities × Cycle Time) ÷ 3600 ÷ Efficiency',
    formulaMM: 'ကြာချိန် (နာရီ) = (မှာယူအရေအတွက် ÷ အပေါက်များ × ပတ်လည်ချိန်) ÷ ၃၆၀၀ ÷ စွမ်းရည်',
    outputUnit: 'hrs',
    selected: false
  }
];

export function FormulaCalculatorRegistration() {
  const [currentStep, setCurrentStep] = useState(1);
  const [showAdvanced, setShowAdvanced] = useState(false);
  const [variables, setVariables] = useState(predefinedVariables);
  const [templates, setTemplates] = useState(formulaTemplates);
  
  // Test calculator state
  const [testInputs, setTestInputs] = useState({
    product: 'Cup-250ml',
    orderQty: '1000',
    machine: 'MCH-01',
    material: 'PP'
  });

  const [testResults, setTestResults] = useState<any>(null);

  const updateVariableValue = (id: string, field: string, value: any) => {
    setVariables(prev => prev.map(v => 
      v.id === id ? { ...v, [field]: value } : v
    ));
  };

  const selectTemplate = (templateId: string) => {
    setTemplates(prev => prev.map(t => 
      ({ ...t, selected: t.id === templateId })
    ));
  };

  const runCalculation = () => {
    // Get selected template
    const selectedTemplate = templates.find(t => t.selected);
    if (!selectedTemplate) {
      return;
    }

    // Get variable values
    const orderQty = parseInt(testInputs.orderQty);
    const cavities = variables.find(v => v.id === 'cavities')?.defaultValue || 4;
    const partWeight = variables.find(v => v.id === 'partWeight')?.defaultValue || 15;
    const runnerWeight = variables.find(v => v.id === 'runnerWeight')?.defaultValue || 12;
    const cycleTime = variables.find(v => v.id === 'cycleTime')?.defaultValue || 30;
    const efficiency = (variables.find(v => v.id === 'efficiency')?.defaultValue || 85) / 100;
    const rejectRate = (variables.find(v => v.id === 'rejectRate')?.defaultValue || 2) / 100;
    const purgeLoss = variables.find(v => v.id === 'purgeLoss')?.defaultValue || 2.5;
    const bagSize = variables.find(v => v.id === 'bagSize')?.defaultValue || 25;

    // Calculate based on selected template
    let result: any = {
      template: selectedTemplate.name,
      assumptions: {
        cavities,
        partWeight,
        runnerWeight,
        cycleTime,
        efficiency: efficiency * 100,
        rejectRate: rejectRate * 100,
        purgeLoss,
        bagSize
      }
    };

    if (selectedTemplate.id === 'material-requirement') {
      const adjustedQty = orderQty / (1 - rejectRate);
      const shotWeight = (partWeight + (runnerWeight / cavities)) / 1000; // Convert to kg
      const materialKg = (adjustedQty / cavities) * shotWeight / efficiency + purgeLoss;
      
      result.materialRequired = {
        kg: Math.round(materialKg * 100) / 100,
        bags: Math.ceil(materialKg / bagSize)
      };
      result.shots = Math.ceil(adjustedQty / cavities);
      result.duration = Math.round(((adjustedQty / cavities) * cycleTime / 3600 / efficiency) * 10) / 10;
    }
    
    else if (selectedTemplate.id === 'bags-required') {
      const adjustedQty = orderQty / (1 - rejectRate);
      const shotWeight = (partWeight + (runnerWeight / cavities)) / 1000;
      const materialKg = (adjustedQty / cavities) * shotWeight / efficiency + purgeLoss;
      const bags = Math.ceil(materialKg / bagSize);
      
      result.bagsRequired = bags;
      result.totalWeight = Math.round(materialKg * 100) / 100;
    }
    
    else if (selectedTemplate.id === 'production-duration') {
      const adjustedQty = orderQty / (1 - rejectRate);
      const totalCycles = Math.ceil(adjustedQty / cavities);
      const durationHours = (totalCycles * cycleTime / 3600) / efficiency;
      
      result.duration = {
        hours: Math.round(durationHours * 10) / 10,
        totalCycles,
        adjustedQty: Math.round(adjustedQty)
      };
    }

    setTestResults(result);
  };

  const renderStepIndicator = () => (
    <div className="flex items-center justify-center mb-8">
      <div className="flex items-center space-x-4">
        {[1, 2, 3].map((step) => (
          <div key={step} className="flex items-center">
            <div 
              className={`w-10 h-10 rounded-full flex items-center justify-center border-2 transition-all ${
                currentStep >= step 
                  ? 'bg-blue-600 border-blue-600 text-white' 
                  : 'border-slate-300 text-slate-400'
              }`}
            >
              {step}
            </div>
            <div className={`ml-3 font-medium ${
              currentStep >= step ? 'text-blue-600' : 'text-slate-400'
            }`}>
              {step === 1 && 'Variables'}
              {step === 2 && 'Formula Templates'}
              {step === 3 && 'Test Calculator'}
            </div>
            {step < 3 && (
              <ChevronRight className={`w-5 h-5 mx-4 ${
                currentStep > step ? 'text-blue-600' : 'text-slate-300'
              }`} />
            )}
          </div>
        ))}
      </div>
    </div>
  );

  const renderStep1 = () => (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-xl font-semibold">Step 1: Variables Setup</h3>
          <p className="text-slate-600">Configure the values used in calculations | တွက်ချက်မှုများတွင်အသုံးပြုသောတန်ဖိုးများကိုပြင်ဆင်ပါ</p>
        </div>
        <div className="flex items-center gap-3">
          <Label htmlFor="advanced-toggle">Advanced Settings</Label>
          <Switch 
            id="advanced-toggle"
            checked={showAdvanced} 
            onCheckedChange={setShowAdvanced} 
          />
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        {variables.map((variable) => (
          <Card key={variable.id} className="p-4">
            <div className="flex items-start gap-3">
              <div className="text-2xl">{variable.icon}</div>
              <div className="flex-1">
                <div className="flex items-center gap-2 mb-2">
                  <h4 className="font-medium">{variable.name}</h4>
                  <Badge variant="outline" className="text-xs">{variable.unit}</Badge>
                </div>
                <p className="text-sm text-slate-500 mb-3">{variable.nameMM}</p>
                
                <div className="space-y-3">
                  <div>
                    <Label>Unit | ယူနစ်</Label>
                    <Select value={variable.unit} onValueChange={(value) => updateVariableValue(variable.id, 'unit', value)}>
                      <SelectTrigger className="mt-1">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="pcs">pcs</SelectItem>
                        <SelectItem value="kg">kg</SelectItem>
                        <SelectItem value="g">g</SelectItem>
                        <SelectItem value="%">%</SelectItem>
                        <SelectItem value="sec">sec</SelectItem>
                        <SelectItem value="count">count</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                  
                  <div>
                    <Label>Default Value | ပုံမှန်တန်ဖိုး</Label>
                    <Input
                      type="number"
                      value={variable.defaultValue}
                      onChange={(e) => updateVariableValue(variable.id, 'defaultValue', parseInt(e.target.value) || 0)}
                      disabled={!variable.editable && !showAdvanced}
                      className="mt-1"
                    />
                  </div>
                  
                  {showAdvanced && (
                    <div>
                      <Label>Notes | မှတ်ချက်များ</Label>
                      <Textarea
                        value={variable.notes}
                        onChange={(e) => updateVariableValue(variable.id, 'notes', e.target.value)}
                        className="mt-1"
                        rows={2}
                      />
                    </div>
                  )}
                </div>
              </div>
            </div>
          </Card>
        ))}
      </div>

      {showAdvanced && (
        <Card className="p-4 border-orange-200 bg-orange-50">
          <div className="flex items-center gap-2 mb-3">
            <Settings className="h-5 w-5 text-orange-600" />
            <h4 className="font-medium text-orange-800">Advanced Settings</h4>
          </div>
          <div className="space-y-3">
            <Button variant="outline" className="gap-2">
              <Plus className="h-4 w-4" />
              Add Custom Variable
            </Button>
            <p className="text-sm text-orange-700">
              Admin users can add custom variables and modify system defaults.
            </p>
          </div>
        </Card>
      )}

      <div className="flex justify-end">
        <Button onClick={() => setCurrentStep(2)} className="gap-2">
          Next: Formula Templates
          <ChevronRight className="h-4 w-4" />
        </Button>
      </div>
    </div>
  );

  const renderStep2 = () => (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h3 className="text-xl font-semibold">Step 2: Formula Templates</h3>
          <p className="text-slate-600">Choose what you want to calculate | တွက်ချက်လိုသည့်အရာကိုရွေးချယ်ပါ</p>
        </div>
        {showAdvanced && (
          <Button variant="outline" className="gap-2">
            <Edit className="h-4 w-4" />
            Advanced Formula Editor
          </Button>
        )}
      </div>

      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {templates.map((template) => (
          <Card 
            key={template.id}
            className={`p-6 cursor-pointer transition-all border-2 ${
              template.selected 
                ? 'border-blue-500 bg-blue-50' 
                : `${template.color} hover:shadow-md`
            }`}
            onClick={() => selectTemplate(template.id)}
          >
            <div className="text-center space-y-4">
              <div className="text-4xl">{template.icon}</div>
              <div>
                <h4 className="font-semibold text-lg">{template.name}</h4>
                <p className="text-sm text-slate-500">{template.nameMM}</p>
              </div>
              <p className="text-sm">{template.description}</p>
              <p className="text-xs text-slate-600">{template.descriptionMM}</p>
              
              <div className="bg-white p-3 rounded-lg">
                <p className="text-sm font-mono">{template.formula}</p>
                <p className="text-xs text-slate-500 mt-1">{template.formulaMM}</p>
              </div>
              
              <Badge className={template.selected ? 'bg-blue-600' : 'bg-slate-400'}>
                Output: {template.outputUnit}
              </Badge>
            </div>
          </Card>
        ))}
      </div>

      {!templates.some(t => t.selected) && (
        <div className="text-center py-8">
          <Info className="h-12 w-12 text-slate-400 mx-auto mb-4" />
          <p className="text-slate-500">Please select a formula template to continue</p>
          <p className="text-sm text-slate-400">ဆက်လက်လုပ်ဆောင်ရန် ဖော်မြူလာတမ်းပလိတ်တစ်ခုကို ရွေးချယ်ပါ</p>
        </div>
      )}

      <div className="flex justify-between">
        <Button variant="outline" onClick={() => setCurrentStep(1)}>
          Previous: Variables
        </Button>
        <Button 
          onClick={() => setCurrentStep(3)} 
          disabled={!templates.some(t => t.selected)}
          className="gap-2"
        >
          Next: Test Calculator
          <ChevronRight className="h-4 w-4" />
        </Button>
      </div>
    </div>
  );

  const renderStep3 = () => (
    <div className="space-y-6">
      <div>
        <h3 className="text-xl font-semibold">Step 3: Test Calculator</h3>
        <p className="text-slate-600">Input your data and get instant results | သင့်ဒေတာများကိုရိုက်ထည့်ပြီး ချက်ချင်းရလဒ်ရယူပါ</p>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Input Panel */}
        <Card className="p-6">
          <h4 className="font-semibold mb-4 flex items-center gap-2">
            <Zap className="h-5 w-5 text-blue-600" />
            Input Data | ဒေတာရိုက်ထည့်ရန်
          </h4>
          
          <div className="space-y-4">
            <div>
              <Label>Product | ထုတ်ကုန်</Label>
              <Select value={testInputs.product} onValueChange={(value) => setTestInputs(prev => ({ ...prev, product: value }))}>
                <SelectTrigger className="mt-1">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="Cup-250ml">Cup 250ml</SelectItem>
                  <SelectItem value="Container-500ml">Container 500ml</SelectItem>
                  <SelectItem value="Lid-Universal">Lid Universal</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>Order Quantity | မှာယူအရေအတွက်</Label>
              <Input
                type="number"
                value={testInputs.orderQty}
                onChange={(e) => setTestInputs(prev => ({ ...prev, orderQty: e.target.value }))}
                className="mt-1"
                placeholder="1000"
              />
            </div>

            <div>
              <Label>Machine | စက်</Label>
              <Select value={testInputs.machine} onValueChange={(value) => setTestInputs(prev => ({ ...prev, machine: value }))}>
                <SelectTrigger className="mt-1">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="MCH-01">MCH-01 (Injection)</SelectItem>
                  <SelectItem value="MCH-02">MCH-02 (Injection)</SelectItem>
                  <SelectItem value="MCH-03">MCH-03 (Extrusion)</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div>
              <Label>Material | ပစ္စည်း</Label>
              <Select value={testInputs.material} onValueChange={(value) => setTestInputs(prev => ({ ...prev, material: value }))}>
                <SelectTrigger className="mt-1">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="PP">PP (Polypropylene)</SelectItem>
                  <SelectItem value="PE">PE (Polyethylene)</SelectItem>
                  <SelectItem value="PS">PS (Polystyrene)</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <Button onClick={runCalculation} className="w-full gap-2 bg-green-600 hover:bg-green-700">
              <Play className="h-4 w-4" />
              Run Calculation | တွက်ချက်မှုလုပ်ဆောင်ရန်
            </Button>
          </div>
        </Card>

        {/* Results Panel */}
        <div className="space-y-4">
          <h4 className="font-semibold flex items-center gap-2">
            <Package className="h-5 w-5 text-green-600" />
            Results | ရလဒ်များ
          </h4>

          {testResults ? (
            <div className="space-y-4">
              {/* Material Results */}
              {testResults.materialRequired && (
                <Card className="p-4 bg-blue-50 border-blue-200">
                  <div className="flex items-center gap-2 mb-3">
                    <div className="text-2xl">📦</div>
                    <h5 className="font-semibold text-blue-800">Material Required</h5>
                  </div>
                  <div className="space-y-2">
                    <div className="flex justify-between">
                      <span>Weight:</span>
                      <strong>{testResults.materialRequired.kg} kg</strong>
                    </div>
                    <div className="flex justify-between">
                      <span>Bags needed:</span>
                      <strong>{testResults.materialRequired.bags} bags</strong>
                    </div>
                  </div>
                </Card>
              )}

              {/* Bags Results */}
              {testResults.bagsRequired && (
                <Card className="p-4 bg-green-50 border-green-200">
                  <div className="flex items-center gap-2 mb-3">
                    <div className="text-2xl">🛍️</div>
                    <h5 className="font-semibold text-green-800">Bags Required</h5>
                  </div>
                  <div className="space-y-2">
                    <div className="flex justify-between">
                      <span>Total bags:</span>
                      <strong>{testResults.bagsRequired} bags</strong>
                    </div>
                    <div className="flex justify-between">
                      <span>Total weight:</span>
                      <strong>{testResults.totalWeight} kg</strong>
                    </div>
                  </div>
                </Card>
              )}

              {/* Duration Results */}
              {testResults.duration && (typeof testResults.duration === 'object') && (
                <Card className="p-4 bg-orange-50 border-orange-200">
                  <div className="flex items-center gap-2 mb-3">
                    <div className="text-2xl">⏱️</div>
                    <h5 className="font-semibold text-orange-800">Production Duration</h5>
                  </div>
                  <div className="space-y-2">
                    <div className="flex justify-between">
                      <span>Duration:</span>
                      <strong>{testResults.duration.hours} hrs</strong>
                    </div>
                    <div className="flex justify-between">
                      <span>Total cycles:</span>
                      <strong>{testResults.duration.totalCycles}</strong>
                    </div>
                  </div>
                </Card>
              )}

              {/* Production Summary */}
              {testResults.shots && (
                <Card className="p-4 bg-purple-50 border-purple-200">
                  <div className="flex items-center gap-2 mb-3">
                    <div className="text-2xl">🏭</div>
                    <h5 className="font-semibold text-purple-800">Production Summary</h5>
                  </div>
                  <div className="space-y-2">
                    <div className="flex justify-between">
                      <span>Total shots:</span>
                      <strong>{testResults.shots}</strong>
                    </div>
                    {testResults.duration && (typeof testResults.duration === 'number') && (
                      <div className="flex justify-between">
                        <span>Duration:</span>
                        <strong>{testResults.duration} hrs</strong>
                      </div>
                    )}
                  </div>
                </Card>
              )}

              {/* Assumptions Used */}
              <Card className="p-4 bg-slate-50">
                <h5 className="font-semibold text-slate-800 mb-3 flex items-center gap-2">
                  <Clock className="h-4 w-4" />
                  Assumptions Used | အသုံးပြုသောယူဆချက်များ
                </h5>
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <div className="flex justify-between">
                    <span>Cavities:</span>
                    <span>{testResults.assumptions.cavities}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Part weight:</span>
                    <span>{testResults.assumptions.partWeight}g</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Runner weight:</span>
                    <span>{testResults.assumptions.runnerWeight}g</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Cycle time:</span>
                    <span>{testResults.assumptions.cycleTime}s</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Efficiency:</span>
                    <span>{testResults.assumptions.efficiency}%</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Reject rate:</span>
                    <span>{testResults.assumptions.rejectRate}%</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Purge loss:</span>
                    <span>{testResults.assumptions.purgeLoss}kg</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Bag size:</span>
                    <span>{testResults.assumptions.bagSize}kg</span>
                  </div>
                </div>
              </Card>
            </div>
          ) : (
            <div className="text-center py-12">
              <Calculator className="h-16 w-16 text-slate-300 mx-auto mb-4" />
              <p className="text-slate-500">Click "Run Calculation" to see results</p>
              <p className="text-sm text-slate-400">ရလဒ်များကြည့်ရန် "တွက်ချက်မှုလုပ်ဆောင်ရန်" ကိုနှိပ်ပါ</p>
            </div>
          )}
        </div>
      </div>

      <div className="flex justify-start">
        <Button variant="outline" onClick={() => setCurrentStep(2)}>
          Previous: Formula Templates
        </Button>
      </div>
    </div>
  );

  return (
    <div className="p-6 space-y-8">
      {/* Header */}
      <div className="text-center">
        <h2 className="text-2xl font-semibold text-slate-900 mb-2">
          Formula & Calculator | ဖော်မြူလာနှင့်တွက်စက်
        </h2>
        <p className="text-slate-600">
          Simple production calculations made easy | ရိုးရှင်းသောထုတ်လုပ်မှုတွက်ချက်မှုများကို လွယ်ကူအောင်ပြုလုပ်
        </p>
      </div>

      {/* Step Indicator */}
      {renderStepIndicator()}

      {/* Step Content */}
      <Card className="border-0 shadow-sm">
        <CardContent className="p-8">
          {currentStep === 1 && renderStep1()}
          {currentStep === 2 && renderStep2()}
          {currentStep === 3 && renderStep3()}
        </CardContent>
      </Card>
    </div>
  );
}