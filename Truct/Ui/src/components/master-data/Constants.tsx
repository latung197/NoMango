import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { Badge } from '../ui/badge';
import { Switch } from '../ui/switch';
import { Alert, AlertDescription } from '../ui/alert';
import { 
  Settings,
  Save,
  RotateCcw,
  Lock,
  Calculator,
  Scale,
  Percent,
  Package
} from 'lucide-react';

// Constants configuration
const defaultConstants = {
  bagWeight: 55, // lb
  gramsPerPound: 454.54, // g/lb
  defaultRejectPercent: 2, // %
  defaultPurgeKg: 2.5, // kg
  useRejectInFormula: false,
  lastUpdatedBy: 'System Admin',
  lastUpdatedOn: '2024-03-15 10:30 AM'
};

export function Constants() {
  const [constants, setConstants] = useState(defaultConstants);
  const [hasChanges, setHasChanges] = useState(false);

  const updateConstant = (key: string, value: any) => {
    setConstants(prev => ({ ...prev, [key]: value }));
    setHasChanges(true);
  };

  const handleSave = () => {
    console.log('Saving constants:', constants);
    setConstants(prev => ({
      ...prev,
      lastUpdatedBy: 'Current User',
      lastUpdatedOn: new Date().toLocaleString()
    }));
    setHasChanges(false);
  };

  const handleReset = () => {
    setConstants(defaultConstants);
    setHasChanges(false);
  };

  const calculateBagKg = () => {
    return (constants.bagWeight * constants.gramsPerPound / 1000).toFixed(2);
  };

  return (
    <div className="p-6 space-y-6">
      {/* Header */}
      <div>
        <h2 className="text-2xl font-semibold text-slate-900">
          Constants | ကိန်းသေများ
        </h2>
        <p className="text-slate-600 mt-1">
          System-wide constants used in production calculations
        </p>
        <p className="text-sm text-slate-500">
          ထုတ်လုပ်မှုတွက်ချက်မှုများတွင်အသုံးပြုသော စနစ်တစ်ခုလုံးအတွက် ကိန်းသေများ
        </p>
      </div>

      {/* Admin Warning */}
      <Alert>
        <Lock className="h-4 w-4" />
        <AlertDescription>
          <strong>Admin Only:</strong> These settings affect all production calculations. Only authorized personnel should modify these values.
          <br />
          <span className="text-xs text-slate-600">
            စီမံခန့်ခွဲသူများသာ: ဤဆက်တင်များသည် ထုတ်လုပ်မှုတွက်ချက်မှုအားလုံးကို ထိခိုက်သည်။ ခွင့်ပြုသောပုဂ္ဂိုလ်များသာ ဤတန်ဖိုးများကို ပြင်ဆင်နိုင်သည်။
          </span>
        </AlertDescription>
      </Alert>

      {/* Constants Configuration */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Material Constants */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Package className="h-5 w-5 text-blue-600" />
              Material Constants | ပစ္စည်းကိန်းသေများ
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Bag Weight */}
            <div>
              <Label htmlFor="bagWeight">Bag Weight (lb) | အိတ်အလေးချိန်</Label>
              <div className="relative mt-1">
                <Input
                  id="bagWeight"
                  type="number"
                  step="0.1"
                  value={constants.bagWeight}
                  onChange={(e) => updateConstant('bagWeight', parseFloat(e.target.value) || 0)}
                  className="pr-8"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-xs text-slate-500">lb</span>
              </div>
              <div className="text-xs text-slate-500 mt-1">
                Standard material bag weight in pounds
              </div>
            </div>

            {/* Grams per Pound */}
            <div>
              <Label htmlFor="gramsPerPound">Grams per Pound | တစ်ပေါင်ဒ်တွင်ရသောဂရမ်</Label>
              <div className="relative mt-1">
                <Input
                  id="gramsPerPound"
                  type="number"
                  step="0.01"
                  value={constants.gramsPerPound}
                  onChange={(e) => updateConstant('gramsPerPound', parseFloat(e.target.value) || 0)}
                  className="pr-12"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-xs text-slate-500">g/lb</span>
              </div>
              <div className="text-xs text-slate-500 mt-1">
                Conversion factor from pounds to grams
              </div>
            </div>

            {/* Calculated Bag Weight in KG */}
            <div className="p-4 bg-green-50 border border-green-200 rounded-lg">
              <div className="flex items-center gap-2 mb-2">
                <Calculator className="h-4 w-4 text-green-600" />
                <Label className="text-green-800">Calculated: 1 Bag Weight (kg)</Label>
              </div>
              <div className="text-2xl font-bold text-green-700">
                {calculateBagKg()} kg
              </div>
              <div className="text-sm text-green-600 mt-1">
                Formula: {constants.bagWeight} lb × {constants.gramsPerPound} g/lb ÷ 1000
              </div>
              <div className="text-xs text-slate-600">
                ဖော်မြူလာ: {constants.bagWeight} ပေါင်ဒ် × {constants.gramsPerPound} ဂရမ်/ပေါင်ဒ် ÷ ၁၀၀၀
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Production Constants */}
        <Card className="border-0 shadow-sm">
          <CardHeader>
            <CardTitle className="flex items-center gap-2">
              <Settings className="h-5 w-5 text-orange-600" />
              Production Constants | ထုတ်လုပ်မှုကိန်းသေများ
            </CardTitle>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Default Reject Percentage */}
            <div>
              <Label htmlFor="defaultRejectPercent">Default Reject % | ပုံမှန်ပယ်ချနှုန်း</Label>
              <div className="relative mt-1">
                <Input
                  id="defaultRejectPercent"
                  type="number"
                  step="0.1"
                  min="0"
                  max="100"
                  value={constants.defaultRejectPercent}
                  onChange={(e) => updateConstant('defaultRejectPercent', parseFloat(e.target.value) || 0)}
                  className="pr-8"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-xs text-slate-500">%</span>
              </div>
              <div className="text-xs text-slate-500 mt-1">
                Default rejection rate for information purposes
              </div>
            </div>

            {/* Default Purge Loss */}
            <div>
              <Label htmlFor="defaultPurgeKg">Default Purge Loss (kg) | ပုံမှန်သန့်စင်ဆုံးရှုံးမှု</Label>
              <div className="relative mt-1">
                <Input
                  id="defaultPurgeKg"
                  type="number"
                  step="0.1"
                  min="0"
                  value={constants.defaultPurgeKg}
                  onChange={(e) => updateConstant('defaultPurgeKg', parseFloat(e.target.value) || 0)}
                  className="pr-8"
                />
                <span className="absolute right-3 top-1/2 transform -translate-y-1/2 text-xs text-slate-500">kg</span>
              </div>
              <div className="text-xs text-slate-500 mt-1">
                Default material loss during machine purging
              </div>
            </div>

            {/* Use Reject in Formula Toggle */}
            <div className="p-4 border rounded-lg">
              <div className="flex items-center justify-between">
                <div>
                  <Label htmlFor="useRejectInFormula">Use Reject % in Base Formula</Label>
                  <div className="text-xs text-slate-500 mt-1">
                    Include reject percentage in standard calculations
                  </div>
                  <div className="text-xs text-slate-400">
                    ပုံမှန်တွက်ချက်မှုများတွင် ပယ်ချနှုန်းကို ထည့်သွင်းခြင်း
                  </div>
                </div>
                <Switch
                  id="useRejectInFormula"
                  checked={constants.useRejectInFormula}
                  onCheckedChange={(checked) => updateConstant('useRejectInFormula', checked)}
                />
              </div>
            </div>

            {/* Formula Preview */}
            <div className="p-4 bg-slate-50 border rounded-lg">
              <div className="text-sm font-medium text-slate-800 mb-2">Current Formula Preview:</div>
              <div className="text-xs font-mono bg-white p-2 rounded border">
                {constants.useRejectInFormula
                  ? `Material = ((Order Qty / (1 - ${constants.defaultRejectPercent}%)) / Cavities) × Weight/Shot + ${constants.defaultPurgeKg}kg`
                  : `Material = (Order Qty / Cavities) × Weight/Shot + ${constants.defaultPurgeKg}kg`
                }
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Usage Examples */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Scale className="h-5 w-5 text-purple-600" />
            Usage Examples | အသုံးပြုမှုဥပမာများ
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="p-4 bg-blue-50 border border-blue-200 rounded-lg">
              <h4 className="font-medium text-blue-800 mb-2">Material Calculation</h4>
              <div className="text-sm text-blue-700">
                • Bag weight: {constants.bagWeight} lb = {calculateBagKg()} kg<br />
                • Purge loss: {constants.defaultPurgeKg} kg per setup<br />
                • Reject rate: {constants.defaultRejectPercent}% (if enabled)
              </div>
            </div>

            <div className="p-4 bg-green-50 border border-green-200 rounded-lg">
              <h4 className="font-medium text-green-800 mb-2">Bag Counting</h4>
              <div className="text-sm text-green-700">
                • Standard bag size: {calculateBagKg()} kg<br />
                • Always rounds up to next whole bag<br />
                • Example: 67.3 kg = 3 bags ({calculateBagKg()} kg each)
              </div>
            </div>

            <div className="p-4 bg-orange-50 border border-orange-200 rounded-lg">
              <h4 className="font-medium text-orange-800 mb-2">Quality Planning</h4>
              <div className="text-sm text-orange-700">
                • Default reject: {constants.defaultRejectPercent}%<br />
                • Can be overridden per product<br />
                • Used for capacity planning only
              </div>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Last Updated Info */}
      <Card className="border-0 shadow-sm">
        <CardContent className="p-4">
          <div className="flex items-center justify-between">
            <div className="text-sm text-slate-600">
              Last updated by <strong>{constants.lastUpdatedBy}</strong> on {constants.lastUpdatedOn}
            </div>
            <div className="flex items-center gap-3">
              {hasChanges && (
                <Badge className="bg-orange-100 text-orange-800">
                  Unsaved Changes
                </Badge>
              )}
              <Button variant="outline" onClick={handleReset} className="gap-2">
                <RotateCcw className="h-4 w-4" />
                Reset | ပြန်လည်သတ်မှတ်ရန်
              </Button>
              <Button onClick={handleSave} className="gap-2" disabled={!hasChanges}>
                <Save className="h-4 w-4" />
                Save Constants | ကိန်းသေများသိမ်းဆည်းရန်
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}