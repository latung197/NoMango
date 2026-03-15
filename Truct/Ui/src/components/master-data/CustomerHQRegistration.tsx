import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from '../ui/card';
import { Badge } from '../ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '../ui/table';
import { Building, MapPin, Phone, User } from 'lucide-react';

const mockCustomers = [
  {
    id: 1,
    customerCode: 'HQ-BKK',
    name: 'Bangkok Headquarters',
    nameMM: 'ဘန်ကောက်ရုံးချုပ်',
    type: 'Internal HQ',
    contact: '+66 2 123 4567',
    address: 'Bangkok, Thailand',
    receivingPerson: 'Mr. Tanakorn',
    receivingPersonMM: 'မစတာတနကွန်း',
    status: 'Active'
  },
  {
    id: 2,
    customerCode: 'EXT-001',
    name: 'Asia Pacific Trading',
    nameMM: 'အာရှပစိဖိတ်ကုန်သွယ်ရေး',
    type: 'External Sales',
    contact: '+65 6789 0123',
    address: 'Singapore',
    receivingPerson: 'Ms. Sarah Lee',
    receivingPersonMM: 'မစ္စဆာရာလီ',
    status: 'Active'
  },
  {
    id: 3,
    customerCode: 'HQ-YGN',
    name: 'Yangon Office',
    nameMM: 'ရန်ကုန်ရုံးခွဲ',
    type: 'Internal HQ',
    contact: '+95 1 234 567',
    address: 'Yangon, Myanmar',
    receivingPerson: 'Ko Thant Zin',
    receivingPersonMM: 'ကိုသန့်ဇင်',
    status: 'Active'
  }
];

export function CustomerHQRegistration() {
  const [customers] = useState(mockCustomers);

  const getTypeColor = (type: string) => {
    return type === 'Internal HQ' 
      ? 'bg-blue-100 text-blue-800 border-blue-200'
      : 'bg-green-100 text-green-800 border-green-200';
  };

  const getTypeIcon = (type: string) => {
    return type === 'Internal HQ' ? Building : MapPin;
  };

  return (
    <div className="p-6 space-y-6">
      {/* Summary Stats */}
      <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-blue-100 rounded-lg">
                <Building className="w-4 h-4 text-blue-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-blue-700">{customers.length}</div>
                <div className="text-sm text-slate-600">Total Customers</div>
                <div className="text-xs text-slate-500">စုစုပေါင်းဖောက်သည်များ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-purple-100 rounded-lg">
                <Building className="w-4 h-4 text-purple-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-purple-700">
                  {customers.filter(c => c.type === 'Internal HQ').length}
                </div>
                <div className="text-sm text-slate-600">Internal HQ</div>
                <div className="text-xs text-slate-500">အတွင်းပိုင်းရုံးချုပ်များ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-green-100 rounded-lg">
                <MapPin className="w-4 h-4 text-green-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-green-700">
                  {customers.filter(c => c.type === 'External Sales').length}
                </div>
                <div className="text-sm text-slate-600">External Sales</div>
                <div className="text-xs text-slate-500">ပြင်ပရောင်းချမှုများ</div>
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="border-0 shadow-sm">
          <CardContent className="p-4">
            <div className="flex items-center gap-3">
              <div className="p-2 bg-orange-100 rounded-lg">
                <User className="w-4 h-4 text-orange-600" />
              </div>
              <div>
                <div className="text-2xl font-bold text-orange-700">
                  {customers.filter(c => c.status === 'Active').length}
                </div>
                <div className="text-sm text-slate-600">Active</div>
                <div className="text-xs text-slate-500">တက်ကြွများ</div>
              </div>
            </div>
          </CardContent>
        </Card>
      </div>

      {/* Main Content */}
      <Card className="border-0 shadow-sm">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Building className="h-5 w-5 text-blue-600" />
            <div>
              <div>Customer & HQ Directory | ဖောက်သည်နှင့်ရုံးချုပ်လမ်းညွှန်</div>
              <div className="text-sm text-slate-500">Internal headquarters and external customers</div>
            </div>
          </CardTitle>
        </CardHeader>

        <CardContent>
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Customer Code | ဖောက်သည်ကုဒ်</TableHead>
                  <TableHead>Name | အမည်</TableHead>
                  <TableHead>Type | အမျိုးအစား</TableHead>
                  <TableHead>Contact | ဆက်သွယ်ရန်</TableHead>
                  <TableHead>Receiver | လက်ခံသူ</TableHead>
                  <TableHead>Status | အခြေအနေ</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {customers.map((customer) => {
                  const TypeIcon = getTypeIcon(customer.type);
                  return (
                    <TableRow key={customer.id}>
                      <TableCell>
                        <Badge variant="outline" className="font-mono">
                          {customer.customerCode}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{customer.name}</div>
                          <div className="text-sm text-slate-500">{customer.nameMM}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center gap-2">
                          <TypeIcon className="h-4 w-4" />
                          <Badge variant="outline" className={getTypeColor(customer.type)}>
                            {customer.type}
                          </Badge>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="flex items-center gap-1 text-sm">
                            <Phone className="h-3 w-3" />
                            {customer.contact}
                          </div>
                          <div className="flex items-center gap-1 text-sm text-slate-500 mt-1">
                            <MapPin className="h-3 w-3" />
                            {customer.address}
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div>
                          <div className="font-medium">{customer.receivingPerson}</div>
                          <div className="text-sm text-slate-500">{customer.receivingPersonMM}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Badge className={
                          customer.status === 'Active' 
                            ? 'bg-green-100 text-green-800' 
                            : 'bg-red-100 text-red-800'
                        }>
                          {customer.status}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <div className="text-sm text-slate-500">Edit</div>
                      </TableCell>
                    </TableRow>
                  );
                })}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}