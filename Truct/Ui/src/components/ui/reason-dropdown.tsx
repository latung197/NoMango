import React, { useState } from 'react';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './select';
import { Button } from './button';
import { Input } from './input';
import { Label } from './label';
import { Textarea } from './textarea';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from './dialog';
import { Badge } from './badge';
import { Plus, CheckCircle, AlertTriangle, Search } from 'lucide-react';

interface ReasonOption {
  id: string;
  en: string;
  mm: string;
  description?: string;
}

interface ReasonDropdownProps {
  value: string;
  onValueChange: (value: string) => void;
  placeholder?: string;
  category: string;
  categoryDisplayName: string;
  categoryDisplayNameMM: string;
  reasons: ReasonOption[];
  onAddReason?: (reason: ReasonOption) => void;
  disabled?: boolean;
  currentUser?: { role: 'admin' | 'user' };
  className?: string;
}

export function ReasonDropdown({
  value,
  onValueChange,
  placeholder = "Select reason | အကြောင်းရင်းရွေးပါ",
  category,
  categoryDisplayName,
  categoryDisplayNameMM,
  reasons = [],
  onAddReason,
  disabled = false,
  currentUser = { role: 'user' },
  className = ""
}: ReasonDropdownProps) {
  const [showAddReasonDialog, setShowAddReasonDialog] = useState(false);
  const [searchTerm, setSearchTerm] = useState("");
  const [newReasonData, setNewReasonData] = useState({
    reasonEn: '',
    reasonMm: '',
    description: ''
  });

  // Filter reasons based on search term
  const filteredReasons = reasons.filter(reason => 
    reason.en.toLowerCase().includes(searchTerm.toLowerCase()) ||
    reason.mm.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const handleAddNewReason = () => {
    if (!newReasonData.reasonEn || !newReasonData.reasonMm) {
      alert('Please fill in both English and Myanmar reason names | အင်္ဂလိပ်နှင့် မြန်မာ အကြောင်းရင်းများကို ဖြည့်ပါ');
      return;
    }

    const newReason: ReasonOption = {
      id: `${category.toUpperCase().substring(0, 3)}${String(Date.now()).slice(-3)}`,
      en: newReasonData.reasonEn,
      mm: newReasonData.reasonMm,
      description: newReasonData.description || undefined
    };

    // Call the parent's add reason handler
    if (onAddReason) {
      onAddReason(newReason);
    }

    // Reset form and close dialog
    setNewReasonData({
      reasonEn: '',
      reasonMm: '',
      description: ''
    });
    setShowAddReasonDialog(false);
    alert('New reason added successfully | အကြောင်းရင်းအသစ် ထည့်သွင်းပြီးပါပြီ');
  };

  const EmptyState = () => (
    <div className="p-6 text-center text-slate-500">
      <div className="w-12 h-12 mx-auto mb-3 rounded-full bg-slate-100 flex items-center justify-center">
        <AlertTriangle className="h-6 w-6 text-slate-400" />
      </div>
      <p className="text-sm font-medium mb-1">No reasons mapped yet</p>
      <p className="text-xs">အကြောင်းရင်းများ မရှိသေးပါ — Master Data မှ ထည့်သွင်းပါ</p>
    </div>
  );

  return (
    <div className={className}>
      <Select value={value} onValueChange={onValueChange} disabled={disabled}>
        <SelectTrigger className="h-12 text-lg">
          <SelectValue placeholder={placeholder} />
        </SelectTrigger>
        <SelectContent className="max-h-80">
          {/* Search functionality */}
          {reasons.length > 5 && (
            <div className="p-2 border-b">
              <div className="relative">
                <Search className="h-4 w-4 absolute left-3 top-3 text-slate-400" />
                <Input
                  placeholder="Search reasons..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-9 h-8"
                />
              </div>
            </div>
          )}

          {/* Reason options */}
          {filteredReasons.length > 0 ? (
            <>
              {filteredReasons.map((reason) => (
                <SelectItem key={reason.id} value={`${reason.en} | ${reason.mm}`}>
                  <div className="py-1">
                    <div className="font-medium">{reason.en}</div>
                    <div className="text-sm text-slate-500">{reason.mm}</div>
                    {reason.description && (
                      <div className="text-xs text-slate-400 mt-1">{reason.description}</div>
                    )}
                  </div>
                </SelectItem>
              ))}
              
              {/* Admin Add Reason Button */}
              {currentUser.role === 'admin' && (
                <>
                  <div className="border-t my-1" />
                  <div className="p-2">
                    <Dialog open={showAddReasonDialog} onOpenChange={setShowAddReasonDialog}>
                      <DialogTrigger asChild>
                        <Button 
                          variant="ghost" 
                          size="sm" 
                          className="w-full justify-start text-blue-600 hover:bg-blue-50 border-dashed border border-blue-300"
                        >
                          <Plus className="h-4 w-4 mr-2" />
                          ➕ Add Reason Code
                        </Button>
                      </DialogTrigger>
                      <DialogContent className="max-w-2xl">
                        <DialogHeader>
                          <DialogTitle>Add Reason Code | အကြောင်းရင်းကုဒ်ထည့်ရန်</DialogTitle>
                          <DialogDescription>
                            Add a new reason code for {categoryDisplayName} category. This will be saved to Master Data and available for all users.
                          </DialogDescription>
                        </DialogHeader>
                        
                        <div className="space-y-6">
                          {/* Category Display */}
                          <div>
                            <Label>Category (အမျိုးအစား)</Label>
                            <div className="p-3 bg-blue-50 rounded-lg border border-blue-200 mt-2">
                              <div className="flex items-center gap-2">
                                <Badge variant="outline" className="font-mono">{category.toUpperCase()}</Badge>
                                <div>
                                  <div className="font-medium text-blue-800">{categoryDisplayName}</div>
                                  <div className="text-sm text-blue-600">{categoryDisplayNameMM}</div>
                                </div>
                              </div>
                            </div>
                          </div>

                          {/* Reason Name (English) */}
                          <div>
                            <Label htmlFor="reasonEn">Reason Name (English) *</Label>
                            <Input
                              id="reasonEn"
                              value={newReasonData.reasonEn}
                              onChange={(e) => setNewReasonData(prev => ({ ...prev, reasonEn: e.target.value }))}
                              placeholder="Enter reason in English (e.g., Temperature sensor fault)"
                              className="h-12 text-lg mt-2"
                            />
                          </div>

                          {/* Reason Name (Myanmar) */}
                          <div>
                            <Label htmlFor="reasonMm">Reason Name (Myanmar) *</Label>
                            <Input
                              id="reasonMm"
                              value={newReasonData.reasonMm}
                              onChange={(e) => setNewReasonData(prev => ({ ...prev, reasonMm: e.target.value }))}
                              placeholder="မြန်မာဘာသာဖြင့် အကြောင်းရင်းရေးပါ (ဥပမာ၊ အပူချိန်အာရုံခံကိရိယာပျက်)"
                              className="h-12 text-lg mt-2"
                            />
                          </div>

                          {/* Description (Optional) */}
                          <div>
                            <Label htmlFor="description">Description (Optional) | ဖော်ပြချက် (မဖြစ်မနေမလိုအပ်)</Label>
                            <Textarea
                              id="description"
                              value={newReasonData.description}
                              onChange={(e) => setNewReasonData(prev => ({ ...prev, description: e.target.value }))}
                              placeholder="Additional details about this reason code..."
                              rows={3}
                              className="mt-2"
                            />
                          </div>

                          {/* Admin Notice */}
                          <div className="bg-amber-50 p-4 rounded-lg border border-amber-200">
                            <div className="flex items-start gap-2">
                              <AlertTriangle className="h-5 w-5 text-amber-600 mt-0.5" />
                              <div>
                                <h4 className="font-medium text-amber-800">Master Data Integration | မာစတာဒေတာ ချိတ်ဆက်မှု</h4>
                                <p className="text-sm text-amber-700 mt-1">
                                  This reason code will be saved to Master Data → Reason Code Registration and available across all modules.
                                </p>
                                <p className="text-xs text-amber-600 mt-1">
                                  ဤအကြောင်းရင်းကုဒ်ကို Master Data → Reason Code Registration တွင်သိမ်းဆည်းပြီး မော်ဂျူးများအားလုံးတွင် အသုံးပြုနိုင်မည်။
                                </p>
                              </div>
                            </div>
                          </div>

                          {/* Action Buttons */}
                          <div className="flex gap-3 pt-4">
                            <Button
                              onClick={handleAddNewReason}
                              disabled={!newReasonData.reasonEn || !newReasonData.reasonMm}
                              className="bg-green-600 hover:bg-green-700 flex-1"
                            >
                              <CheckCircle className="h-4 w-4 mr-2" />
                              Save to Master Data | Master Data သို့သိမ်း
                            </Button>
                            <Button 
                              variant="outline" 
                              onClick={() => {
                                setShowAddReasonDialog(false);
                                setNewReasonData({ reasonEn: '', reasonMm: '', description: '' });
                                setSearchTerm("");
                              }}
                            >
                              Cancel | ပယ်ဖျက်
                            </Button>
                          </div>
                        </div>
                      </DialogContent>
                    </Dialog>
                  </div>
                </>
              )}
            </>
          ) : (
            <>
              <EmptyState />
              {/* Admin Add Reason Button for empty state */}
              {currentUser.role === 'admin' && (
                <div className="p-2 border-t">
                  <Dialog open={showAddReasonDialog} onOpenChange={setShowAddReasonDialog}>
                    <DialogTrigger asChild>
                      <Button 
                        variant="outline" 
                        size="sm" 
                        className="w-full border-dashed border-blue-300 text-blue-600 hover:bg-blue-50"
                      >
                        <Plus className="h-4 w-4 mr-2" />
                        ➕ Add First Reason Code
                      </Button>
                    </DialogTrigger>
                    <DialogContent className="max-w-2xl">
                      <DialogHeader>
                        <DialogTitle>Add Reason Code | အကြောင်းရင်းကုဒ်ထည့်ရန်</DialogTitle>
                        <DialogDescription>
                          Add a new reason code for {categoryDisplayName} category. This will be saved to Master Data and available for all users.
                        </DialogDescription>
                      </DialogHeader>
                      
                      <div className="space-y-6">
                        {/* Category Display */}
                        <div>
                          <Label>Category (အမျိုးအစား)</Label>
                          <div className="p-3 bg-blue-50 rounded-lg border border-blue-200 mt-2">
                            <div className="flex items-center gap-2">
                              <Badge variant="outline" className="font-mono">{category.toUpperCase()}</Badge>
                              <div>
                                <div className="font-medium text-blue-800">{categoryDisplayName}</div>
                                <div className="text-sm text-blue-600">{categoryDisplayNameMM}</div>
                              </div>
                            </div>
                          </div>
                        </div>

                        {/* Reason Name (English) */}
                        <div>
                          <Label htmlFor="reasonEn">Reason Name (English) *</Label>
                          <Input
                            id="reasonEn"
                            value={newReasonData.reasonEn}
                            onChange={(e) => setNewReasonData(prev => ({ ...prev, reasonEn: e.target.value }))}
                            placeholder="Enter reason in English (e.g., Temperature sensor fault)"
                            className="h-12 text-lg mt-2"
                          />
                        </div>

                        {/* Reason Name (Myanmar) */}
                        <div>
                          <Label htmlFor="reasonMm">Reason Name (Myanmar) *</Label>
                          <Input
                            id="reasonMm"
                            value={newReasonData.reasonMm}
                            onChange={(e) => setNewReasonData(prev => ({ ...prev, reasonMm: e.target.value }))}
                            placeholder="မြန်မာဘာသာဖြင့် အကြောင်းရင်းရေးပါ (ဥပမာ၊ အပူချိန်အာရုံခံကိရိယာပျက်)"
                            className="h-12 text-lg mt-2"
                          />
                        </div>

                        {/* Description (Optional) */}
                        <div>
                          <Label htmlFor="description">Description (Optional) | ဖော်ပြချက် (မဖြစ်မနေမလိုအပ်)</Label>
                          <Textarea
                            id="description"
                            value={newReasonData.description}
                            onChange={(e) => setNewReasonData(prev => ({ ...prev, description: e.target.value }))}
                            placeholder="Additional details about this reason code..."
                            rows={3}
                            className="mt-2"
                          />
                        </div>

                        {/* Admin Notice */}
                        <div className="bg-amber-50 p-4 rounded-lg border border-amber-200">
                          <div className="flex items-start gap-2">
                            <AlertTriangle className="h-5 w-5 text-amber-600 mt-0.5" />
                            <div>
                              <h4 className="font-medium text-amber-800">Master Data Integration | မာစတာဒေတာ ချိတ်ဆက်မှု</h4>
                              <p className="text-sm text-amber-700 mt-1">
                                This reason code will be saved to Master Data → Reason Code Registration and available across all modules.
                              </p>
                              <p className="text-xs text-amber-600 mt-1">
                                ဤအကြောင်းရင်းကုဒ်ကို Master Data → Reason Code Registration တွင်သိမ်းဆည်းပြီး မော်ဂျူးများအားလုံးတွင် အသုံးပြုနိုင်မည်။
                              </p>
                            </div>
                          </div>
                        </div>

                        {/* Action Buttons */}
                        <div className="flex gap-3 pt-4">
                          <Button
                            onClick={handleAddNewReason}
                            disabled={!newReasonData.reasonEn || !newReasonData.reasonMm}
                            className="bg-green-600 hover:bg-green-700 flex-1"
                          >
                            <CheckCircle className="h-4 w-4 mr-2" />
                            Save to Master Data | Master Data သို့သိမ်း
                          </Button>
                          <Button 
                            variant="outline" 
                            onClick={() => {
                              setShowAddReasonDialog(false);
                              setNewReasonData({ reasonEn: '', reasonMm: '', description: '' });
                              setSearchTerm("");
                            }}
                          >
                            Cancel | ပယ်ဖျက်
                          </Button>
                        </div>
                      </div>
                    </DialogContent>
                  </Dialog>
                </div>
              )}
            </>
          )}
        </SelectContent>
      </Select>


    </div>
  );
}