import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Button } from '../ui/button';
import { Badge } from '../ui/badge';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '../ui/tabs';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { AlertTriangle, Clock, XCircle, ArrowRight, Settings } from 'lucide-react';

// Mock data for reason codes
const mockReasonCodes = {
  downtime: [
    {
      id: 1,
      code: 'DT-01',
      description: 'Power Failure',
      descriptionMM: 'လျှပ်စစ်ဓာတ်အားပြတ်တောက်မှု',
      category: 'Downtime',
      defaultAction: 'Pause',
      status: 'Active'
    },
    {
      id: 2,
      code: 'DT-02',
      description: 'Machine Breakdown',
      descriptionMM: 'စက်ပျက်စီးမှု',
      category: 'Downtime',
      defaultAction: 'Stop',
      status: 'Active'
    }
  ],
  defect: [
    {
      id: 3,
      code: 'DEF-01',
      description: 'Color Variation',
      descriptionMM: 'အရောင်ကွဲပြားမှု',
      category: 'Defect',
      defaultAction: 'Flag',
      status: 'Active'
    },
    {
      id: 4,
      code: 'DEF-02',
      description: 'Dimension Error',
      descriptionMM: 'အရွယ်အစားအမှား',
      category: 'Defect',
      defaultAction: 'Flag',
      status: 'Active'
    }
  ],
  transfer: [
    {
      id: 5,
      code: 'TR-01',
      description: 'Warehouse Transfer',
      descriptionMM: 'ကုန်လှောင်ရုံသို့လွှဲပြောင်းမှု',
      category: 'Transfer',
      defaultAction: 'Flag',
      status: 'Active'
    }
  ],
  adjustment: [
    {
      id: 6,
      code: 'ADJ-01',
      description: 'Stock Count Adjustment',
      descriptionMM: 'စတော့ရေတွက်ညှိမှု',
      category: 'Adjustment',
      defaultAction: 'Flag',
      status: 'Active'
    }
  ]
};

export function ReasonCodeRegistration() {
  const [activeTab, setActiveTab] = useState('downtime');
  const [reasonCodes] = useState(mockReasonCodes);

  const getCategoryIcon = (category: string) => {
    switch (category.toLowerCase()) {
      case 'downtime':
        return <Clock className="h-4 w-4" />;
      case 'defect':
        return <XCircle className="h-4 w-4" />;
      case 'transfer':
        return <ArrowRight className="h-4 w-4" />;
      case 'adjustment':
        return <Settings className="h-4 w-4" />;
      default:
        return <AlertTriangle className="h-4 w-4" />;
    }
  };

  const getCategoryColor = (category: string) => {
    switch (category.toLowerCase()) {
      case 'downtime':
        return 'text-red-600 bg-red-100';
      case 'defect':
        return 'text-orange-600 bg-orange-100';
      case 'transfer':
        return 'text-blue-600 bg-blue-100';
      case 'adjustment':
        return 'text-purple-600 bg-purple-100';
      default:
        return 'text-slate-600 bg-slate-100';
    }
  };

  const getCurrentData = () => {
    return reasonCodes[activeTab as keyof typeof reasonCodes] || [];
  };

  return (
    <div className="p-6 space-y-6">
      {/* Summary Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-red-100 rounded-lg">
                <Clock className="w-4 h-4 text-red-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-red-700">{reasonCodes.downtime.length}</div>
                <div className="text-sm text-slate-600">Downtime Codes</div>
                <div className="text-xs text-slate-500">စက်ရပ်ချိန်ကုဒ်များ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-orange-100 rounded-lg">
                <XCircle className="w-4 h-4 text-orange-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-orange-700">{reasonCodes.defect.length}</div>
                <div className="text-sm text-slate-600">Defect Codes</div>
                <div className="text-xs text-slate-500">ချို့ယွင်းမှုကုဒ်များ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-blue-100 rounded-lg">
                <ArrowRight className="w-4 h-4 text-blue-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-blue-700">{reasonCodes.transfer.length}</div>
                <div className="text-sm text-slate-600">Transfer Codes</div>
                <div className="text-xs text-slate-500">လွှဲပြောင်းကုဒ်များ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-purple-100 rounded-lg">
                <Settings className="w-4 h-4 text-purple-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-purple-700">{reasonCodes.adjustment.length}</div>
                <div className="text-sm text-slate-600">Adjustment Codes</div>
                <div className="text-xs text-slate-500">ညှိမှုကုဒ်များ</div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Main Content with Tabs */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <AlertTriangle className="h-5 w-5 text-red-600" />
            <div>
              <div>Reason Code Directory | အကြောင်းရင်းကုဒ်လမ်းညွှန်</div>
              <div className="text-sm text-slate-500">System reason codes for operations tracking</div>
            </div>
          </CardTitle>
        </CardHeader>

        <CardContent>
          <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
            <div className="flex items-center justify-between mb-6">
              <TabsList className="grid w-full max-w-md grid-cols-4">
                <TabsTrigger value="downtime" className="gap-2">
                  <Clock className="h-4 w-4" />
                  Downtime
                </TabsTrigger>
                <TabsTrigger value="defect" className="gap-2">
                  <XCircle className="h-4 w-4" />
                  Defect
                </TabsTrigger>
                <TabsTrigger value="transfer" className="gap-2">
                  <ArrowRight className="h-4 w-4" />
                  Transfer
                </TabsTrigger>
                <TabsTrigger value="adjustment" className="gap-2">
                  <Settings className="h-4 w-4" />
                  Adjustment
                </TabsTrigger>
              </TabsList>
              
              <Button className="gap-2 bg-red-600 hover:bg-red-700">
                <AlertTriangle className="h-4 w-4" />
                Add Reason Code | ကုဒ်အသစ်ထည့်ရန်
              </Button>
            </div>

            <TabsContent value="downtime" className="mt-0">
              <ReasonCodeTable data={getCurrentData()} category="Downtime" />
            </TabsContent>
            
            <TabsContent value="defect" className="mt-0">
              <ReasonCodeTable data={getCurrentData()} category="Defect" />
            </TabsContent>
            
            <TabsContent value="transfer" className="mt-0">
              <ReasonCodeTable data={getCurrentData()} category="Transfer" />
            </TabsContent>
            
            <TabsContent value="adjustment" className="mt-0">
              <ReasonCodeTable data={getCurrentData()} category="Adjustment" />
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>
    </div>
  );
}

function ReasonCodeTable({ data, category }: { data: any[], category: string }) {
  const getCategoryIcon = (category: string) => {
    switch (category.toLowerCase()) {
      case 'downtime':
        return <Clock className="h-4 w-4 text-red-600" />;
      case 'defect':
        return <XCircle className="h-4 w-4 text-orange-600" />;
      case 'transfer':
        return <ArrowRight className="h-4 w-4 text-blue-600" />;
      case 'adjustment':
        return <Settings className="h-4 w-4 text-purple-600" />;
      default:
        return <AlertTriangle className="h-4 w-4 text-slate-600" />;
    }
  };

  const getActionColor = (action: string) => {
    switch (action.toLowerCase()) {
      case 'pause':
        return 'bg-yellow-100 text-yellow-800';
      case 'stop':
        return 'bg-red-100 text-red-800';
      case 'flag':
        return 'bg-blue-100 text-blue-800';
      default:
        return 'bg-slate-100 text-slate-800';
    }
  };

  return (
    <div className="overflow-x-auto">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead>Code | ကုဒ်</TableHead>
            <TableHead>Description | ဖော်ပြချက်</TableHead>
            <TableHead>Category | အမျိုးအစား</TableHead>
            <TableHead>Default Action | ပုံမှန်လုပ်ဆောင်ချက်</TableHead>
            <TableHead>Status | အခြေအနေ</TableHead>
            <TableHead>Actions</TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {data.map((item) => (
            <TableRow key={item.id}>
              <TableCell>
                <Badge variant="outline" className="font-mono">
                  {item.code}
                </Badge>
              </TableCell>
              <TableCell>
                <div>
                  <div className="font-medium">{item.description}</div>
                  <div className="text-sm text-slate-500">{item.descriptionMM}</div>
                </div>
              </TableCell>
              <TableCell>
                <div className="flex items-center gap-2">
                  {getCategoryIcon(item.category)}
                  <span>{item.category}</span>
                </div>
              </TableCell>
              <TableCell>
                <Badge className={getActionColor(item.defaultAction)}>
                  {item.defaultAction}
                </Badge>
              </TableCell>
              <TableCell>
                <Badge className={
                  item.status === 'Active' 
                    ? 'bg-green-100 text-green-800' 
                    : 'bg-red-100 text-red-800'
                }>
                  {item.status}
                </Badge>
              </TableCell>
              <TableCell>
                <Button
                  variant="outline"
                  size="sm"
                  className="gap-1"
                >
                  Edit
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </div>
  );
}