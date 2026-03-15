import React, { useState } from 'react';
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogFooter } from './ui/dialog';
import { Button } from './ui/button';
import { Label } from './ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Avatar, AvatarFallback } from './ui/avatar';
import { Sun, Moon, CalendarCheck } from 'lucide-react';
import { toast } from 'sonner';

interface Machine {
  id: string;
  name: string;
  status: 'running' | 'idle' | 'down' | 'unassigned';
}

interface Operator {
  id: string;
  name: string;
  nameLocal: string;
  status: 'free' | 'busy' | 'absent';
}

interface Assignment {
  id: number;
  machineId: string;
  operatorId: string;
  operatorName: string;
  shift: 'day' | 'night';
  date: string;
  duration?: string;
}

interface AssignmentCalendarModalProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  machines: Machine[];
  operators: Operator[];
  scheduledAssignments: Assignment[];
  onAssignmentSave: (assignments: Assignment[]) => void;
}

interface SelectedCell {
  machineId: string;
  date: Date;
}

export function AssignmentCalendarModal({
  open,
  onOpenChange,
  machines,
  operators,
  scheduledAssignments,
  onAssignmentSave
}: AssignmentCalendarModalProps) {
  const [calendarView, setCalendarView] = useState<'week' | 'month'>('week');
  const [selectedCells, setSelectedCells] = useState<SelectedCell[]>([]);
  const [selectedOperatorId, setSelectedOperatorId] = useState('');
  const [selectedShift, setSelectedShift] = useState<'day' | 'night'>('day');
  const [localAssignments, setLocalAssignments] = useState<Assignment[]>(scheduledAssignments);

  // Generate week dates for calendar
  const generateWeekDates = () => {
    const dates = [];
    const today = new Date();
    const currentDay = today.getDay();
    const diff = currentDay === 0 ? -6 : 1 - currentDay; // Start from Monday
    
    for (let i = 0; i < 7; i++) {
      const date = new Date(today);
      date.setDate(today.getDate() + diff + i);
      dates.push(date);
    }
    return dates;
  };

  // Generate month dates for calendar (full month grid)
  const generateMonthDates = () => {
    const dates = [];
    const today = new Date();
    const year = today.getFullYear();
    const month = today.getMonth();
    
    // Get first day of month and last day of month
    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);
    
    // Get the day of week for the first day (0 = Sunday, 1 = Monday, etc.)
    const firstDayOfWeek = firstDay.getDay();
    const startOffset = firstDayOfWeek === 0 ? -6 : 1 - firstDayOfWeek; // Start from Monday
    
    // Calculate total weeks needed
    const totalDays = lastDay.getDate();
    const weeksNeeded = Math.ceil((totalDays + Math.abs(startOffset)) / 7);
    
    // Generate all dates for the month view
    for (let week = 0; week < weeksNeeded; week++) {
      const weekDates = [];
      for (let day = 0; day < 7; day++) {
        const date = new Date(year, month, 1);
        date.setDate(date.getDate() + startOffset + (week * 7) + day);
        weekDates.push(date);
      }
      dates.push(weekDates);
    }
    
    return dates;
  };

  // Handle single cell click (toggle selection)
  const handleCellClick = (machine: Machine, date: Date) => {
    const cellKey = `${machine.id}-${date.toISOString()}`;
    
    // Check if this cell is already selected
    const isSelected = selectedCells.some(
      cell => cell.machineId === machine.id && 
      cell.date.toISOString() === date.toISOString()
    );

    if (isSelected) {
      // Deselect
      setSelectedCells(prev => prev.filter(
        cell => !(cell.machineId === machine.id && 
        cell.date.toISOString() === date.toISOString())
      ));
    } else {
      // Select
      setSelectedCells(prev => [...prev, { machineId: machine.id, date }]);
    }
  };

  // Handle "Assign All Week/Month" button click
  const handleAssignAllPeriod = (machine: Machine) => {
    const dates = calendarView === 'week' ? generateWeekDates() : generateMonthDates().flat();
    
    // Filter to only dates in the current month for month view
    const filteredDates = calendarView === 'month' 
      ? dates.filter(d => d.getMonth() === new Date().getMonth())
      : dates;
    
    // Add all dates for this machine to selectedCells
    const newCells: SelectedCell[] = filteredDates.map(date => ({
      machineId: machine.id,
      date
    }));

    // Remove any existing selections for this machine and add new ones
    setSelectedCells(prev => {
      const withoutMachine = prev.filter(cell => cell.machineId !== machine.id);
      return [...withoutMachine, ...newCells];
    });

    toast.success(`Selected all ${calendarView} for ${machine.name}`);
  };

  // Handle save assignment for selected cells
  const handleSaveAssignment = () => {
    if (selectedCells.length === 0 || !selectedOperatorId) {
      toast.error('Please select cells and operator');
      return;
    }
    
    const operator = operators.find(op => op.id === selectedOperatorId);
    if (!operator) return;

    const newAssignments: Assignment[] = [];

    selectedCells.forEach(cell => {
      const newAssignment: Assignment = {
        id: Date.now() + Math.random(),
        machineId: cell.machineId,
        operatorId: operator.id,
        operatorName: operator.name,
        shift: selectedShift,
        date: cell.date.toISOString()
      };
      newAssignments.push(newAssignment);
    });

    // Remove existing assignments for the same machine/dates and add new ones
    const updatedAssignments = localAssignments.filter(a => {
      return !newAssignments.some(na => 
        na.machineId === a.machineId &&
        new Date(na.date).toDateString() === new Date(a.date).toDateString()
      );
    });

    const finalAssignments = [...updatedAssignments, ...newAssignments];
    setLocalAssignments(finalAssignments);
    onAssignmentSave(finalAssignments);
    
    toast.success(`✅ ${operator.name} assigned to ${selectedCells.length} slot(s)`);
    
    // Clear selections
    setSelectedCells([]);
    setSelectedOperatorId('');
  };

  // Clear all selections
  const handleClearSelections = () => {
    setSelectedCells([]);
  };

  // Check if a cell is selected
  const isCellSelected = (machineId: string, date: Date) => {
    return selectedCells.some(
      cell => cell.machineId === machineId && 
      cell.date.toDateString() === date.toDateString()
    );
  };

  // Get the machine names for selected cells
  const getSelectedMachineNames = () => {
    const machineIds = [...new Set(selectedCells.map(c => c.machineId))];
    return machineIds.map(id => machines.find(m => m.id === id)?.name || id).join(', ');
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-[96vw] max-h-[90vh] overflow-hidden flex flex-col p-0" aria-describedby={undefined}>
        <DialogHeader className="px-6 pt-6 pb-3">
          <div className="flex items-center justify-between">
            <DialogTitle className="text-xl">Assignment Calendar | အော်ပရေတာဇယား</DialogTitle>
            <div className="flex items-center gap-2 bg-slate-100 rounded-lg p-1">
              <Button
                variant={calendarView === 'week' ? 'default' : 'ghost'}
                size="sm"
                onClick={() => {
                  setCalendarView('week');
                  setSelectedCells([]);
                }}
                className={calendarView === 'week' ? 'bg-black hover:bg-black text-white' : ''}
              >
                Week
              </Button>
              <Button
                variant={calendarView === 'month' ? 'default' : 'ghost'}
                size="sm"
                onClick={() => {
                  setCalendarView('month');
                  setSelectedCells([]);
                }}
                className={calendarView === 'month' ? 'bg-black hover:bg-black text-white' : ''}
              >
                Month
              </Button>
            </div>
          </div>
        </DialogHeader>
        
        <div className="flex-1 overflow-y-auto px-4 py-2">
          {calendarView === 'week' ? (
            // WEEK VIEW - Table Layout - ALL 7 DAYS VISIBLE
            <table className="w-full border-collapse table-fixed">
              <thead>
                <tr>
                  <th className="p-2 text-left font-semibold border-b bg-white text-sm" style={{width: '140px'}}>
                    Machine
                  </th>
                  {generateWeekDates().map((date, idx) => (
                    <th key={idx} className="p-2 text-center border-b text-sm">
                      <div className="font-semibold">{date.toLocaleDateString('en-US', { weekday: 'short' })}</div>
                      <div className="text-xs text-slate-500">{date.toLocaleDateString('en-US', { month: 'short', day: 'numeric' })}</div>
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {machines.map((machine) => (
                  <tr key={machine.id} className="border-b hover:bg-slate-50">
                    <td className="p-2 bg-white border-r" style={{width: '140px'}}>
                      <div className="flex flex-col gap-1.5">
                        <div>
                          <div className="font-semibold text-xs">{machine.id}</div>
                          <div className="text-xs text-slate-500 truncate">{machine.name}</div>
                        </div>
                        <Button
                          size="sm"
                          onClick={() => handleAssignAllPeriod(machine)}
                          className="bg-gradient-to-r from-blue-500 to-green-500 hover:from-blue-600 hover:to-green-600 text-white rounded-full px-2 py-1 text-xs gap-1 h-7"
                        >
                          <CalendarCheck className="h-3 w-3" />
                          All Week
                        </Button>
                      </div>
                    </td>
                    {generateWeekDates().map((date, idx) => {
                      const assignment = localAssignments.find(
                        a => a.machineId === machine.id && 
                        new Date(a.date).toDateString() === date.toDateString()
                      );
                      const isSelected = isCellSelected(machine.id, date);
                      
                      return (
                        <td 
                          key={idx} 
                          className="p-1.5"
                        >
                          {assignment ? (
                            <div 
                              className={`p-1.5 rounded text-center cursor-pointer transition-all ${
                                isSelected ? 'ring-2 ring-blue-500' : ''
                              } ${
                                assignment.shift === 'day' 
                                  ? 'bg-blue-100 border border-blue-300' 
                                  : 'bg-indigo-100 border border-indigo-300'
                              }`}
                              onClick={() => handleCellClick(machine, date)}
                            >
                              <div className="flex items-center justify-center mb-0.5">
                                <Avatar className="h-5 w-5">
                                  <AvatarFallback className="text-xs bg-white">
                                    {assignment.operatorName[0]}
                                  </AvatarFallback>
                                </Avatar>
                              </div>
                              <div className="text-xs font-medium truncate">{assignment.operatorName.split(' ')[0]}</div>
                              <div className="flex items-center justify-center gap-0.5 mt-0.5">
                                {assignment.shift === 'day' ? (
                                  <><Sun className="h-3 w-3 text-amber-600" /><span className="text-xs">Day</span></>
                                ) : (
                                  <><Moon className="h-3 w-3 text-indigo-600" /><span className="text-xs">Night</span></>
                                )}
                              </div>
                            </div>
                          ) : (
                            <div 
                              className={`border-2 border-dashed rounded p-2 text-center text-xs cursor-pointer transition-all hover:border-blue-400 hover:bg-blue-50 h-[80px] flex items-center justify-center ${
                                isSelected 
                                  ? 'border-blue-500 bg-blue-100 font-medium' 
                                  : 'border-slate-300 text-slate-400'
                              }`}
                              onClick={() => handleCellClick(machine, date)}
                            >
                              {isSelected ? '✓ Selected' : 'Click to assign'}
                            </div>
                          )}
                        </td>
                      );
                    })}
                  </tr>
                ))}
              </tbody>
            </table>
          ) : (
            // MONTH VIEW - Grid Layout (Different UI)
            <div className="space-y-6">
              {machines.map((machine) => (
                <div key={machine.id} className="border border-slate-200 rounded-lg overflow-hidden">
                  <div className="bg-slate-100 p-4 flex items-center justify-between">
                    <div>
                      <div className="font-semibold">{machine.id}</div>
                      <div className="text-sm text-slate-500">{machine.name}</div>
                    </div>
                    <Button
                      size="sm"
                      onClick={() => handleAssignAllPeriod(machine)}
                      className="bg-gradient-to-r from-blue-500 to-green-500 hover:from-blue-600 hover:to-green-600 text-white rounded-full px-4 py-2 text-sm gap-2"
                    >
                      <CalendarCheck className="h-4 w-4" />
                      Assign All Month
                    </Button>
                  </div>
                  
                  <div className="p-4">
                    {/* Calendar Header */}
                    <div className="grid grid-cols-7 gap-2 mb-2">
                      {['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'].map(day => (
                        <div key={day} className="text-center text-sm font-semibold text-slate-600 p-2">
                          {day}
                        </div>
                      ))}
                    </div>
                    
                    {/* Calendar Grid */}
                    {generateMonthDates().map((week, weekIdx) => (
                      <div key={weekIdx} className="grid grid-cols-7 gap-2 mb-2">
                        {week.map((date, dayIdx) => {
                          const isCurrentMonth = date.getMonth() === new Date().getMonth();
                          const assignment = localAssignments.find(
                            a => a.machineId === machine.id && 
                            new Date(a.date).toDateString() === date.toDateString()
                          );
                          const isSelected = isCellSelected(machine.id, date);
                          
                          return (
                            <div 
                              key={dayIdx}
                              className={`aspect-square ${!isCurrentMonth ? 'opacity-30' : ''}`}
                            >
                              {assignment ? (
                                <div 
                                  className={`h-full p-2 rounded-lg text-center cursor-pointer transition-all ${
                                    isSelected ? 'ring-2 ring-blue-500' : ''
                                  } ${
                                    assignment.shift === 'day' 
                                      ? 'bg-blue-100 border-2 border-blue-300' 
                                      : 'bg-indigo-100 border-2 border-indigo-300'
                                  }`}
                                  onClick={() => isCurrentMonth && handleCellClick(machine, date)}
                                >
                                  <div className="font-semibold text-sm">{date.getDate()}</div>
                                  <div className="text-xs truncate">{assignment.operatorName.split(' ')[0]}</div>
                                  {assignment.shift === 'day' ? (
                                    <Sun className="h-3 w-3 mx-auto text-amber-600 mt-1" />
                                  ) : (
                                    <Moon className="h-3 w-3 mx-auto text-indigo-600 mt-1" />
                                  )}
                                </div>
                              ) : (
                                <div 
                                  className={`h-full border-2 rounded-lg text-center cursor-pointer transition-all p-2 ${
                                    isSelected 
                                      ? 'border-blue-500 bg-blue-100' 
                                      : 'border-slate-200 hover:border-blue-400 hover:bg-blue-50'
                                  } ${!isCurrentMonth ? 'cursor-not-allowed' : ''}`}
                                  onClick={() => isCurrentMonth && handleCellClick(machine, date)}
                                >
                                  <div className="text-sm font-medium text-slate-700">
                                    {date.getDate()}
                                  </div>
                                  {isSelected && isCurrentMonth && (
                                    <div className="text-xs text-blue-600 mt-1">✓</div>
                                  )}
                                </div>
                              )}
                            </div>
                          );
                        })}
                      </div>
                    ))}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>

        {selectedCells.length > 0 && (
          <div className="border-t pt-3 bg-blue-50 rounded-lg p-4 mx-4 mb-2">
            <div className="flex items-center justify-between mb-3">
              <h4 className="font-semibold">
                Quick Assign - {selectedCells.length} slot(s) selected
              </h4>
              <Button 
                variant="outline" 
                size="sm"
                onClick={handleClearSelections}
              >
                Clear Selection
              </Button>
            </div>
            
            <div className="text-sm text-slate-600 mb-3">
              Machines: {getSelectedMachineNames()}
            </div>
            
            <div className="grid grid-cols-4 gap-3">
              <div>
                <Label className="text-xs font-medium mb-1 block">Operator</Label>
                <Select value={selectedOperatorId} onValueChange={setSelectedOperatorId}>
                  <SelectTrigger className="bg-white h-9">
                    <SelectValue placeholder="Select..." />
                  </SelectTrigger>
                  <SelectContent>
                    {operators.filter(op => op.status !== 'absent').map(op => (
                      <SelectItem key={op.id} value={op.id}>
                        <div className="flex items-center gap-2">
                          <Avatar className="h-4 w-4">
                            <AvatarFallback className="text-xs">{op.name[0]}</AvatarFallback>
                          </Avatar>
                          {op.name}
                        </div>
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label className="text-xs font-medium mb-1 block">Shift</Label>
                <Select value={selectedShift} onValueChange={(val: any) => setSelectedShift(val)}>
                  <SelectTrigger className="bg-white h-9">
                    <SelectValue />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="day">
                      <div className="flex items-center gap-2">
                        <Sun className="h-3.5 w-3.5" />
                        Day Shift
                      </div>
                    </SelectItem>
                    <SelectItem value="night">
                      <div className="flex items-center gap-2">
                        <Moon className="h-3.5 w-3.5" />
                        Night Shift
                      </div>
                    </SelectItem>
                  </SelectContent>
                </Select>
              </div>

              <div>
                <Label className="text-xs font-medium mb-1 block">Selected Dates</Label>
                <div className="h-9 px-2 bg-white rounded border text-sm flex items-center">
                  {selectedCells.length} day(s)
                </div>
              </div>

              <div className="flex items-end">
                <Button 
                  className="w-full bg-blue-600 hover:bg-blue-700 h-9"
                  disabled={!selectedOperatorId}
                  onClick={handleSaveAssignment}
                >
                  Save Assignment
                </Button>
              </div>
            </div>
          </div>
        )}

        <DialogFooter className="px-6 pb-4">
          <Button variant="outline" onClick={() => onOpenChange(false)}>
            Close
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}