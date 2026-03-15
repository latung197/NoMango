import React, { useState, useCallback, useEffect, useRef } from 'react';
import { Card, CardHeader, CardContent } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Breadcrumb, BreadcrumbItem, BreadcrumbLink, BreadcrumbList, BreadcrumbPage, BreadcrumbSeparator } from './ui/breadcrumb';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from './ui/dialog';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Textarea } from './ui/textarea';
import { Label } from './ui/label';
import { Input } from './ui/input';
import { Calendar } from './ui/calendar';
import { Popover, PopoverContent, PopoverTrigger } from './ui/popover';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from './ui/tooltip';
import { ContextMenu, ContextMenuContent, ContextMenuItem, ContextMenuTrigger, ContextMenuSeparator } from './ui/context-menu';
import { ImageWithFallback } from './figma/ImageWithFallback';
import { useJobSchedule, ProductionPlan as SharedProductionPlan, MoldChangeRequest, checkMoldChange, productMoldMapping, machineMoldAssignments, moldInfo } from './JobScheduleContext';
import { toast } from 'sonner';
import { 
  ArrowLeft, 
  Calendar as CalendarIcon, 
  Clock, 
  ExternalLink,
  Edit,
  Trash2,
  Target,
  User,
  Package,
  Play,
  Pause,
  AlertTriangle,
  Activity,
  ChevronLeft,
  ChevronRight,
  GripVertical,
  Split,
  CheckCircle2,
  ArrowRightLeft,
  Settings
} from 'lucide-react';
import { format, addDays, startOfWeek, endOfWeek, eachDayOfInterval, isSameDay, addWeeks, subWeeks, addMonths, subMonths, startOfMonth, endOfMonth, eachWeekOfInterval } from 'date-fns';

// Use the shared ProductionPlan interface from context
type ProductionPlan = SharedProductionPlan;

interface Machine {
  id: string;
  name: string;
  nameMyanmar: string;
  status: 'running' | 'idle' | 'down' | 'maintenance';
  capacity: number;
  efficiency: number;
  cycleTime: number;
}

interface MachineCalendarViewProps {
  machineId: string;
  onBack: () => void;
  onOpenInNewTab?: () => void;
  mode?: 'planning' | 'production-control';
  breadcrumbContext?: string;
}

const mockMachines: Record<string, Machine> = {
  'IM-001': {
    id: 'IM-001',
    name: 'Injection Machine 1',
    nameMyanmar: 'ထောက်ပံ့စက် ၁',
    status: 'running',
    capacity: 1200,
    efficiency: 87,
    cycleTime: 12
  },
  'IM-002': {
    id: 'IM-002',
    name: 'Injection Machine 2',
    nameMyanmar: 'ထောက်ပံ့စက် ၂',
    status: 'idle',
    capacity: 1500,
    efficiency: 92,
    cycleTime: 10
  },
  'IM-003': {
    id: 'IM-003',
    name: 'Injection Machine 3',
    nameMyanmar: 'ထောက်ပံ့စက် ၃',
    status: 'running',
    capacity: 800,
    efficiency: 94,
    cycleTime: 15
  },
  'IM-004': {
    id: 'IM-004',
    name: 'Injection Machine 4',
    nameMyanmar: 'ထောက်ပံ့စက် ၄',
    status: 'down',
    capacity: 1000,
    efficiency: 45,
    cycleTime: 18
  },
  'IM-005': {
    id: 'IM-005',
    name: 'Injection Machine 5',
    nameMyanmar: 'ထောက်ပံ့စက် ၅',
    status: 'maintenance',
    capacity: 1300,
    efficiency: 0,
    cycleTime: 14
  },
  'IM-006': {
    id: 'IM-006',
    name: 'Injection Machine 6',
    nameMyanmar: 'ထောက်ပံ့စက် ၆',
    status: 'running',
    capacity: 2000,
    efficiency: 89,
    cycleTime: 8
  }
};

// Machine compatibility matrix - defines which machines are compatible with each other
const machineCompatibility: Record<string, string[]> = {
  'IM-001': ['IM-002', 'IM-003', 'IM-006'], // Large capacity machines
  'IM-002': ['IM-001', 'IM-003', 'IM-006'], // Large capacity machines  
  'IM-003': ['IM-001', 'IM-002', 'IM-004'], // Medium capacity machines
  'IM-004': ['IM-003', 'IM-005'], // Medium capacity machines
  'IM-005': ['IM-004', 'IM-006'], // Maintenance-compatible machines
  'IM-006': ['IM-001', 'IM-002', 'IM-005'] // High capacity machines
};

// Get compatible machines for a given machine
const getCompatibleMachines = (machineId: string): string[] => {
  return machineCompatibility[machineId] || [];
};

// Helper function to generate consistent plan IDs
const generatePlanId = (machineId: string, productId: string, date: Date, status: string, index: number): string => {
  const dateKey = format(date, 'yyyy-MM-dd');
  // Use a deterministic hash based on content for stable plan IDs
  return `plan-${machineId}-${productId}-${dateKey}-${index}`;
};

// Base plan templates - same structure but status depends on date
const generateDateAwarePlans = (machineId: string, selectedDate: Date): ProductionPlan[] => {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  const targetDate = new Date(selectedDate);
  targetDate.setHours(0, 0, 0, 0);
  
  const daysDiff = Math.floor((targetDate.getTime() - today.getTime()) / (1000 * 60 * 60 * 24));
  
  // Base templates for each machine - same products, different random times
  const machineTemplates: Record<string, Array<{
    productId: string,
    productName: string,
    productImage: string,
    quantity: number,
    operatorName: string,
    materialRequirements: { glue: number, color: string, bags: number }
  }>> = {
    'IM-001': [
      {
        productId: 'PRD-006',
        productName: 'Disposable Plate 9inch',
        productImage: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?w=100&h=100&fit=crop',
        quantity: 4200,
        operatorName: 'Kyaw Kyaw',
        materialRequirements: { glue: 6.3, color: 'White', bags: 10 }
      },
      {
        productId: 'PRD-007',
        productName: 'Lunch Box Kids',
        productImage: 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=100&h=100&fit=crop',
        quantity: 2800,
        operatorName: 'Ma Ma',
        materialRequirements: { glue: 9.5, color: 'Multi-Color', bags: 12 }
      },
      {
        productId: 'PRD-001',
        productName: 'Plastic Cup 16oz Clear',
        productImage: 'https://images.unsplash.com/photo-1558618047-3c8c76ca7d13?w=100&h=100&fit=crop',
        quantity: 5000,
        operatorName: 'Thant Zin',
        materialRequirements: { glue: 7.8, color: 'Clear', bags: 9 }
      },
      {
        productId: 'PRD-005',
        productName: 'Water Bottle 500ml',
        productImage: 'https://images.unsplash.com/photo-1571068316344-75bc76f77890?w=100&h=100&fit=crop',
        quantity: 6000,
        operatorName: 'Win Htut',
        materialRequirements: { glue: 8.7, color: 'Transparent', bags: 12 }
      }
    ],
    'IM-002': [
      {
        productId: 'PRD-006',
        productName: 'Disposable Plate 9inch',
        productImage: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?w=100&h=100&fit=crop',
        quantity: 4200,
        operatorName: 'Kyaw Kyaw',
        materialRequirements: { glue: 6.3, color: 'White', bags: 10 }
      },
      {
        productId: 'PRD-007',
        productName: 'Lunch Box Kids',
        productImage: 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=100&h=100&fit=crop',
        quantity: 2800,
        operatorName: 'Ma Ma',
        materialRequirements: { glue: 9.5, color: 'Multi-Color', bags: 12 }
      },
      {
        productId: 'PRD-008',
        productName: 'Plastic Spoon Set',
        productImage: 'https://images.unsplash.com/photo-1558618047-3c8c76ca7d13?w=100&h=100&fit=crop',
        quantity: 9500,
        operatorName: 'Ko Ko',
        materialRequirements: { glue: 4.2, color: 'White', bags: 6 }
      },
      {
        productId: 'PRD-009',
        productName: 'Bottle 1L Green',
        productImage: 'https://images.unsplash.com/photo-1571068316344-75bc76f77890?w=100&h=100&fit=crop',
        quantity: 3600,
        operatorName: 'Zaw Zaw',
        materialRequirements: { glue: 11.3, color: 'Green', bags: 14 }
      }
    ],
    'IM-003': [
      {
        productId: 'PRD-006',
        productName: 'Disposable Plate 9inch',
        productImage: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?w=100&h=100&fit=crop',
        quantity: 4200,
        operatorName: 'Kyaw Kyaw',
        materialRequirements: { glue: 6.3, color: 'White', bags: 10 }
      },
      {
        productId: 'PRD-007',
        productName: 'Lunch Box Kids',
        productImage: 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=100&h=100&fit=crop',
        quantity: 2800,
        operatorName: 'Ma Ma',
        materialRequirements: { glue: 9.5, color: 'Multi-Color', bags: 12 }
      },
      {
        productId: 'PRD-001',
        productName: 'Plastic Cup 16oz Clear',
        productImage: 'https://images.unsplash.com/photo-1558618047-3c8c76ca7d13?w=100&h=100&fit=crop',
        quantity: 6200,
        operatorName: 'Zaw Zaw',
        materialRequirements: { glue: 7.8, color: 'Clear', bags: 9 }
      },
      {
        productId: 'PRD-002',
        productName: 'Food Container 1L White',
        productImage: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?w=100&h=100&fit=crop',
        quantity: 4100,
        operatorName: 'Mya Mya',
        materialRequirements: { glue: 15.2, color: 'White', bags: 18 }
      }
    ],
    'IM-004': [
      {
        productId: 'PRD-006',
        productName: 'Disposable Plate 9inch',
        productImage: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?w=100&h=100&fit=crop',
        quantity: 4200,
        operatorName: 'Kyaw Kyaw',
        materialRequirements: { glue: 6.3, color: 'White', bags: 10 }
      },
      {
        productId: 'PRD-007',
        productName: 'Lunch Box Kids',
        productImage: 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=100&h=100&fit=crop',
        quantity: 2800,
        operatorName: 'Ma Ma',
        materialRequirements: { glue: 9.5, color: 'Multi-Color', bags: 12 }
      },
      {
        productId: 'PRD-005',
        productName: 'Water Bottle 500ml',
        productImage: 'https://images.unsplash.com/photo-1571068316344-75bc76f77890?w=100&h=100&fit=crop',
        quantity: 7200,
        operatorName: 'Nyi Nyi',
        materialRequirements: { glue: 8.7, color: 'Transparent', bags: 12 }
      },
      {
        productId: 'PRD-003',
        productName: 'Bottle Cap 28mm Blue',
        productImage: 'https://images.unsplash.com/photo-1582719471384-894fbb16e074?w=100&h=100&fit=crop',
        quantity: 9200,
        operatorName: 'Thin Thin',
        materialRequirements: { glue: 5.2, color: 'Blue', bags: 8 }
      }
    ],
    'IM-005': [
      {
        productId: 'PRD-006',
        productName: 'Disposable Plate 9inch',
        productImage: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?w=100&h=100&fit=crop',
        quantity: 4200,
        operatorName: 'Kyaw Kyaw',
        materialRequirements: { glue: 6.3, color: 'White', bags: 10 }
      },
      {
        productId: 'PRD-007',
        productName: 'Lunch Box Kids',
        productImage: 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=100&h=100&fit=crop',
        quantity: 2800,
        operatorName: 'Ma Ma',
        materialRequirements: { glue: 9.5, color: 'Multi-Color', bags: 12 }
      },
      {
        productId: 'PRD-009',
        productName: 'Bottle 1L Green',
        productImage: 'https://images.unsplash.com/photo-1571068316344-75bc76f77890?w=100&h=100&fit=crop',
        quantity: 4600,
        operatorName: 'Phyu Phyu',
        materialRequirements: { glue: 11.3, color: 'Green', bags: 14 }
      },
      {
        productId: 'PRD-002',
        productName: 'Food Container 1L White',
        productImage: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?w=100&h=100&fit=crop',
        quantity: 5600,
        operatorName: 'Win Htut',
        materialRequirements: { glue: 15.2, color: 'White', bags: 18 }
      }
    ],
    'IM-006': [
      {
        productId: 'PRD-006',
        productName: 'Disposable Plate 9inch',
        productImage: 'https://images.unsplash.com/photo-1584464491033-06628f3a6b7b?w=100&h=100&fit=crop',
        quantity: 4200,
        operatorName: 'Kyaw Kyaw',
        materialRequirements: { glue: 6.3, color: 'White', bags: 10 }
      },
      {
        productId: 'PRD-007',
        productName: 'Lunch Box Kids',
        productImage: 'https://images.unsplash.com/photo-1553062407-98eeb64c6a62?w=100&h=100&fit=crop',
        quantity: 2800,
        operatorName: 'Ma Ma',
        materialRequirements: { glue: 9.5, color: 'Multi-Color', bags: 12 }
      },
      {
        productId: 'PRD-001',
        productName: 'Plastic Cup 16oz Clear',
        productImage: 'https://images.unsplash.com/photo-1558618047-3c8c76ca7d13?w=100&h=100&fit=crop',
        quantity: 5524,
        operatorName: 'Ko Ko',
        materialRequirements: { glue: 7.8, color: 'Clear', bags: 9 }
      },
      {
        productId: 'PRD-008',
        productName: 'Plastic Spoon Set',
        productImage: 'https://images.unsplash.com/photo-1558618047-3c8c76ca7d13?w=100&h=100&fit=crop',
        quantity: 6500,
        operatorName: 'Htet Htet',
        materialRequirements: { glue: 4.2, color: 'White', bags: 6 }
      }
    ]
  };

  // Realistic time slots aligned with machine operating schedules
  const machineTimeSlots: Record<string, string[][]> = {
    'IM-001': [['08:00', '12:00'], ['13:00', '16:00'], ['17:00', '21:00'], ['22:00', '02:00']], // 8 AM - 4 PM main shift
    'IM-002': [['07:00', '11:00'], ['12:00', '15:00'], ['16:00', '20:00'], ['21:00', '01:00']], // 7 AM - 3 PM main shift  
    'IM-003': [['09:00', '12:00'], ['13:00', '17:00'], ['18:00', '22:00'], ['23:00', '03:00']], // 9 AM - 5 PM main shift
    'IM-004': [['14:00', '18:00'], ['19:00', '22:00'], ['23:00', '03:00'], ['04:00', '08:00']], // 2 PM - 10 PM main shift (afternoon)
    'IM-005': [['10:00', '14:00'], ['15:00', '18:00'], ['19:00', '23:00'], ['00:00', '04:00']], // 10 AM - 6 PM main shift
    'IM-006': [['06:00', '10:00'], ['11:00', '14:00'], ['15:00', '19:00'], ['20:00', '00:00']]  // 6 AM - 2 PM main shift (early)
  };

  const templates = machineTemplates[machineId] || machineTemplates['IM-001'];
  const timeSlots = machineTimeSlots[machineId] || machineTimeSlots['IM-001'];
  
  // Create more realistic plan distribution based on factory workflow
  const generateRealisticPlans = () => {
    const plans: any[] = [];
    const dateKey = `${machineId}-${targetDate.toISOString().split('T')[0]}`;
    const dayHash = dateKey.split('').reduce((hash, char) => hash + char.charCodeAt(0), 0);
    const dayOfWeek = targetDate.getDay(); // 0 = Sunday, 6 = Saturday
    
    // Reduce planning on weekends (but still some production for continuous operations)
    const isWeekend = dayOfWeek === 0 || dayOfWeek === 6;
    const weekendFactor = isWeekend ? 0.3 : 1.0;
    
    if (daysDiff < 0) {
      // ALL past dates - ONLY completed jobs (no running jobs in the past)
      let completedCount;
      if (daysDiff < -7) {
        // Week+ ago - fewer jobs, some gaps
        completedCount = Math.max(1, Math.floor(((dayHash % 3) + 1) * weekendFactor));
      } else if (daysDiff < -3) {
        // Several days ago - moderate activity
        completedCount = Math.max(1, Math.floor(((dayHash % 4) + 2) * weekendFactor));
      } else {
        // Recent past (1-3 days ago) - higher activity
        completedCount = Math.max(2, Math.floor(((dayHash % 3) + 3) * weekendFactor));
      }
      
      for (let i = 0; i < completedCount && i < templates.length; i++) {
        plans.push({
          ...templates[i],
          status: 'completed' as const, // ALWAYS completed for past dates
          startTime: timeSlots[i % timeSlots.length][0],
          endTime: timeSlots[i % timeSlots.length][1],
          jobId: `JOB-${machineId}-${Math.abs(daysDiff)}-${i + 1}`,
          operatorName: templates[i].operatorName
        });
      }
    } else if (daysDiff === 0) {
      // Today - realistic mix based on current time
      const now = new Date();
      const currentHour = now.getHours();
      const currentMinute = now.getMinutes();
      const currentTimeInMinutes = currentHour * 60 + currentMinute;
      
      // Ensure we have at least one running job during operating hours (7 AM - 10 PM)
      let hasRunningJob = false;
      const isOperatingHours = currentHour >= 7 && currentHour < 22;
      
      for (let i = 0; i < templates.length; i++) {
        const timeSlot = timeSlots[i % timeSlots.length];
        const startTime = timeSlot[0];
        const endTime = timeSlot[1];
        
        const [startHour, startMin] = startTime.split(':').map(Number);
        let [endHour, endMin] = endTime.split(':').map(Number);
        
        // Handle overnight shifts (end time < start time)
        if (endHour < startHour) {
          endHour += 24;
        }
        
        const slotStartInMinutes = startHour * 60 + startMin;
        const slotEndInMinutes = endHour * 60 + endMin;
        
        let status: ProductionPlan['status'];
        
        // Determine status based on current time
        if (slotEndInMinutes <= currentTimeInMinutes) {
          status = 'completed';
        } else if (slotStartInMinutes <= currentTimeInMinutes && currentTimeInMinutes < slotEndInMinutes) {
          status = 'running';
          hasRunningJob = true;
        } else if (slotStartInMinutes > currentTimeInMinutes) {
          status = 'approved'; // Scheduled for later today
        } else {
          continue; // Edge case - skip
        }
        
        plans.push({
          ...templates[i],
          status,
          startTime,
          endTime,
          jobId: status !== 'draft' ? `JOB-${machineId}-${format(targetDate, 'MMdd')}-${i + 1}` : undefined,
          operatorName: status !== 'draft' ? templates[i].operatorName : undefined
        });
      }
      
      // If no running job found during operating hours, create one artificially
      if (!hasRunningJob && isOperatingHours && plans.length > 0) {
        // Find the most recent completed job and make it running instead
        for (let i = plans.length - 1; i >= 0; i--) {
          if (plans[i].status === 'completed') {
            plans[i].status = 'running';
            // Extend the end time to current + 2 hours
            const extendedEndHour = Math.min(currentHour + 2, 23);
            plans[i].endTime = `${extendedEndHour.toString().padStart(2, '0')}:00`;
            break;
          }
        }
      }
    } else if (daysDiff === 1) {
      // Tomorrow - mostly approved with some finals
      const approvedCount = Math.floor(((dayHash % 3) + 2) * weekendFactor);
      for (let i = 0; i < templates.length; i++) {
        let status: ProductionPlan['status'];
        if (i < approvedCount) {
          status = 'approved';
        } else if (i < approvedCount + 1) {
          status = 'final';
        } else {
          status = 'draft';
        }
        
        plans.push({
          ...templates[i],
          status,
          startTime: status !== 'draft' ? timeSlots[i % timeSlots.length][0] : '',
          endTime: status !== 'draft' ? timeSlots[i % timeSlots.length][1] : '',
          jobId: status === 'approved' ? `JOB-${machineId}-${format(targetDate, 'MMdd')}-${String(i + 1).padStart(3, '0')}` : undefined,
          operatorName: status === 'approved' ? templates[i].operatorName : status === 'final' ? templates[i].operatorName : undefined
        });
      }
    } else if (daysDiff <= 7) {
      // Next week - mix but less scheduled, vary by machine type
      const machineEfficiency = {
        'IM-001': 0.8, // High volume machine - more planned
        'IM-002': 0.7, // Medium volume
        'IM-003': 0.6, // Lower volume 
        'IM-004': 0.4, // Specialty - less regular
        'IM-005': 0.5, // Mixed production
        'IM-006': 0.9  // High efficiency small items
      };
      
      const efficiency = machineEfficiency[machineId as keyof typeof machineEfficiency] || 0.6;
      const planCount = Math.max(1, Math.floor((dayHash % 4) * efficiency * weekendFactor + 1));
      const approvedCount = Math.floor(planCount * 0.6);
      const finalCount = Math.floor(planCount * 0.3);
      
      for (let i = 0; i < planCount && i < templates.length; i++) {
        let status: ProductionPlan['status'];
        if (i < approvedCount) {
          status = 'approved';
        } else if (i < approvedCount + finalCount) {
          status = 'final';
        } else {
          status = 'draft';
        }
        
        plans.push({
          ...templates[i],
          status,
          startTime: status !== 'draft' ? timeSlots[i % timeSlots.length][0] : '',
          endTime: status !== 'draft' ? timeSlots[i % timeSlots.length][1] : '',
          jobId: status === 'approved' ? `JOB-${machineId}-${format(targetDate, 'MMdd')}-${String(i + 1).padStart(3, '0')}` : undefined,
          operatorName: status === 'approved' ? templates[i].operatorName : status === 'final' ? templates[i].operatorName : undefined
        });
      }
    } else {
      // Far future - create consistent unscheduled plans for dates that show counts in month view
      const planningHorizon = {
        'IM-001': 4, // High volume - planned further ahead
        'IM-002': 3, 
        'IM-003': 3, 
        'IM-004': 5, // Specialty - needs more lead time
        'IM-005': 3,
        'IM-006': 2  // Fast turnaround items
      };
      
      const horizon = planningHorizon[machineId as keyof typeof planningHorizon] || 3;
      
      // Create consistent planning based on date to match month view counts
      const dateKey = format(targetDate, 'yyyy-MM-dd');
      const dateSpecificSeed = Array.from(dateKey).reduce((sum, char) => sum + char.charCodeAt(0), 0);
      
      // Ensure certain dates have unscheduled plans to match month view
      const shouldHavePlans = (dateSpecificSeed % 3 === 0) || (daysDiff >= 2 && daysDiff <= 8); // Sept 27-Oct 3 range
      
      if (shouldHavePlans) {
        // Calculate plan count based on date for consistency
        let planCount = 1;
        if (daysDiff === 2 || daysDiff === 3) planCount = 1; // Sept 27, 28 = 1 unscheduled
        else if (daysDiff === 4 || daysDiff === 5) planCount = 2; // Sept 30, Oct 1 = 2 unscheduled  
        else if (daysDiff === 6) planCount = 1; // Oct 2 = 1 unscheduled
        else planCount = Math.max(1, Math.floor((dateSpecificSeed % 3) + 1)); // Other dates
        
        for (let i = 0; i < planCount && i < templates.length; i++) {
          // Create a mix of scheduled and unscheduled plans
          const shouldBeUnscheduled = i >= Math.floor(planCount * 0.6); // 40% unscheduled
          const status = shouldBeUnscheduled ? 'draft' : (daysDiff > 14 ? 'draft' : 'final');
          
          plans.push({
            ...templates[i],
            status: status as const,
            startTime: (status === 'final' && !shouldBeUnscheduled) ? timeSlots[i % timeSlots.length][0] : '',
            endTime: (status === 'final' && !shouldBeUnscheduled) ? timeSlots[i % timeSlots.length][1] : '',
            jobId: undefined,
            operatorName: status === 'final' ? templates[i].operatorName : undefined
          });
        }
      }
    }
    
    return plans;
  };
  
  const realisticPlans = generateRealisticPlans();
  
  return realisticPlans.map((planData, index) => {
    const { status, startTime, endTime, jobId, operatorName, ...template } = planData;

    return {
      id: generatePlanId(machineId, template.productId, selectedDate, status, index),
      productId: template.productId,
      productName: template.productName,
      productImage: template.productImage,
      quantity: template.quantity,
      startTime,
      endTime,
      date: selectedDate,
      status,
      operatorName,
      jobId,
      machineId, // Add machineId to track which machine this plan belongs to
      plannerName: 'Production Planner',
      materialRequirements: template.materialRequirements
    };
  });
};


// COMPLETELY FIXED Draggable Plan Component - All Scenarios Covered
const DraggablePlan = React.forwardRef<HTMLDivElement, { 
  plan: ProductionPlan; 
  children: React.ReactNode;
  mode?: 'planning' | 'production-control';
}>(({ plan, children, mode = 'planning' }, ref) => {
  // Universal draggability - works for both scheduled and unscheduled plans
  const isDraggable = mode === 'planning' 
    ? (plan.status === 'draft' || plan.status === 'final')
    : (plan.status === 'approved' && plan.jobId);
  
  const [isDragging, setIsDragging] = useState(false);
  const isScheduled = !!(plan.startTime && plan.endTime);
  
  const handleDragStart = (e: React.DragEvent<HTMLDivElement>) => {
    if (!isDraggable) {
      console.log('❌ Drag prevented - not draggable:', plan.productName, plan.status, 'Mode:', mode);
      e.preventDefault();
      return;
    }
    
    console.log('✨ DRAG START - Universal:', {
      planId: plan.id,
      productName: plan.productName,
      status: plan.status,
      isScheduled: isScheduled,
      startTime: plan.startTime || 'UNSCHEDULED',
      endTime: plan.endTime || 'UNSCHEDULED',
      mode: mode,
      action: isScheduled ? 'Can reschedule or unschedule' : 'Can schedule',
      location: isScheduled ? 'TIMELINE' : 'SIDEBAR',
      machineId: plan.machineId
    });
    
    setIsDragging(true);
    
    // Clean, complete drag data - ensure all properties are preserved
    const completePlan = {
      id: plan.id,
      productId: plan.productId,
      productName: plan.productName,
      productImage: plan.productImage,
      quantity: plan.quantity,
      startTime: plan.startTime || '',
      endTime: plan.endTime || '',
      date: plan.date,
      status: plan.status,
      operatorName: plan.operatorName || '',
      jobId: plan.jobId || '',
      machineId: plan.machineId,
      plannerName: plan.plannerName || '',
      materialRequirements: plan.materialRequirements,
      // Add debugging metadata
      _dragTimestamp: Date.now(),
      _isDraggable: isDraggable,
      _isScheduled: isScheduled,
      _mode: mode
    };
    
    console.log('📝 Setting complete drag data:', completePlan);
    
    // Set reliable drag data with fallback
    try {
      e.dataTransfer.setData('text/plain', plan.id);
      e.dataTransfer.setData('application/json', JSON.stringify(completePlan));
      e.dataTransfer.effectAllowed = 'move';
      
      // Clean drag image
      e.dataTransfer.setDragImage(e.currentTarget, 50, 25);
      
      console.log('📝 Drag data set successfully for:', plan.productName);
    } catch (error) {
      console.error('❌ Error setting drag data:', error);
      // Fallback - just set text data
      e.dataTransfer.setData('text', plan.id);
      e.dataTransfer.effectAllowed = 'move';
    }
  };

  const handleDragEnd = () => {
    console.log('✅ DRAG END - Plan:', plan.id);
    setIsDragging(false);
  };

  if (!isDraggable) {
    return (
      <div ref={ref} className="cursor-not-allowed opacity-60" title={`Cannot drag: ${plan.status} status not allowed in ${mode} mode`}>
        {children}
      </div>
    );
  }

  const dragTitle = isScheduled 
    ? `Drag to reschedule on timeline or unschedule to sidebar` 
    : `Drag to timeline to schedule`;

  return (
    <div 
      ref={ref}
      draggable={true}
      onDragStart={handleDragStart}
      onDragEnd={handleDragEnd}
      className={`cursor-move select-none transition-all duration-200 ${
        isDragging ? 'opacity-50 scale-95' : 'opacity-100 hover:scale-[1.02]'
      } hover:shadow-lg`}
      style={{ userSelect: 'none' }}
      title={dragTitle}
    >
      {children}
    </div>
  );
});

DraggablePlan.displayName = 'DraggablePlan';

// Fixed Droppable Time Slot for Planning Dashboard - Always Allow in Planning Mode
function DroppableTimeSlot({ hour, date, children, onDrop, mode = 'planning' }: { 
  hour: number; 
  date: Date; 
  children: React.ReactNode;
  onDrop: (plan: ProductionPlan, hour: number, date?: Date, isFromAnotherMachine?: boolean) => void;
  mode?: 'planning' | 'production-control';
}) {
  const [isOver, setIsOver] = useState(false);
  const [canDrop, setCanDrop] = useState(false);

  const handleDragEnter = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsOver(true);
    // For planning mode, always allow drop initially
    if (mode === 'planning') {
      setCanDrop(true);
    }
  };

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    
    // For planning mode, be permissive and don't do complex validation during dragOver
    if (mode === 'planning') {
      setCanDrop(true);
      e.dataTransfer.dropEffect = 'move';
      return;
    }
    
    // For production control, do minimal validation to prevent false negatives
    setCanDrop(true);
    e.dataTransfer.dropEffect = 'move';
  };

  const handleDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
    const rect = e.currentTarget.getBoundingClientRect();
    const x = e.clientX;
    const y = e.clientY;
    
    if (x < rect.left || x > rect.right || y < rect.top || y > rect.bottom) {
      setIsOver(false);
      setCanDrop(false);
    }
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsOver(false);
    setCanDrop(false);
    
    const planData = e.dataTransfer.getData('application/json');
    const planId = e.dataTransfer.getData('text/plain');
    
    console.log('💫 DROPPABLE TIME SLOT - Drop at hour:', hour, 'Plan ID:', planId, 'Mode:', mode, 'Date:', format(date, 'yyyy-MM-dd'));
    
    if (planData && planId) {
      try {
        const plan = JSON.parse(planData) as ProductionPlan;
        console.log('📦 DROPPABLE TIME SLOT - Plan details:', {
          id: plan.id,
          productName: plan.productName,
          status: plan.status,
          currentTimes: `${plan.startTime}-${plan.endTime}`,
          jobId: plan.jobId,
          isCurrentlyScheduled: !!(plan.startTime && plan.endTime)
        });
        
        // Quick validation - let the main handler do the detailed work
        let isValidDrop = true;
        let reason = '';
        
        if (mode === 'planning') {
          if (plan.status !== 'draft' && plan.status !== 'final') {
            isValidDrop = false;
            reason = `Only draft and final plans can be scheduled (current: ${plan.status})`;
          }
        } else {
          if (plan.status !== 'approved' || !plan.jobId) {
            isValidDrop = false;
            reason = `Only approved jobs can be scheduled (current: ${plan.status})`;
          }
        }
        
        if (isValidDrop) {
          console.log('✅ DROPPABLE TIME SLOT - Valid drop, calling main handler');
          onDrop(plan, hour, date, false);
        } else {
          console.log('❌ DROPPABLE TIME SLOT - Invalid drop:', reason);
          toast.error('Cannot schedule here', {
            description: reason,
            duration: 3000,
          });
        }
      } catch (error) {
        console.error('❌ DROPPABLE TIME SLOT - Parsing error:', error);
        toast.error('Error scheduling plan', { 
          description: 'Could not parse plan data',
          duration: 2000 
        });
      }
    } else {
      console.log('❌ DROPPABLE TIME SLOT - Missing data - planData:', !!planData, 'planId:', planId);
      toast.error('Missing plan data', { 
        description: 'Please try dragging again',
        duration: 2000 
      });
    }
  };

  return (
    <div 
      onDragEnter={handleDragEnter}
      onDragOver={handleDragOver}
      onDragLeave={handleDragLeave}
      onDrop={handleDrop}
      className={`relative transition-all duration-200 ${
        isOver ? (canDrop ? 'bg-green-100 border-green-300 border-2 border-dashed' : 'bg-red-100 border-red-300 border-2 border-dashed') : ''
      }`}
    >
      {children}
      {isOver && canDrop && (
        <div className="absolute inset-0 flex items-center justify-center bg-green-50/90 rounded-lg z-10 pointer-events-none">
          <div className="text-green-700 font-medium">Drop to schedule at {hour.toString().padStart(2, '0')}:00</div>
        </div>
      )}
      {isOver && !canDrop && (
        <div className="absolute inset-0 flex items-center justify-center bg-red-50/90 rounded-lg z-10 pointer-events-none">
          <div className="text-red-700 font-medium flex items-center gap-1">
            <AlertTriangle className="h-4 w-4" />
            Cannot schedule here
          </div>
        </div>
      )}
    </div>
  );
}

// COMPLETELY FIXED Droppable Sidebar for Planning Dashboard - Step by Step
function DroppableSidebar({ children, onDrop, mode = 'planning' }: { 
  children: React.ReactNode;
  onDrop: (plan: ProductionPlan) => void;
  mode?: 'planning' | 'production-control';
}) {
  const [isOver, setIsOver] = useState(false);
  const [canDrop, setCanDrop] = useState(false);

  const handleDragEnter = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    console.log('🎯 SIDEBAR - Drag Enter - Mode:', mode);
    setIsOver(true);
    
    // Be permissive for both modes during drag enter
    setCanDrop(true);
    console.log('✅ SIDEBAR - Allowing drag enter for mode:', mode);
  };

  const handleDragOver = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    
    // For planning mode, always allow drops and validate only on actual drop
    if (mode === 'planning') {
      setCanDrop(true);
      e.dataTransfer.dropEffect = 'move';
      console.log('🎯 SIDEBAR - DragOver Planning Mode - allowing drop');
      return;
    }
    
    // For production control, do basic validation
    try {
      // Note: getData during dragOver is limited in some browsers, so we're permissive here
      console.log('🎯 SIDEBAR - DragOver Production Mode');
      setCanDrop(true);
      e.dataTransfer.dropEffect = 'move';
    } catch (error) {
      console.warn('🎯 SIDEBAR - DragOver error:', error);
      setCanDrop(false);
      e.dataTransfer.dropEffect = 'none';
    }
  };

  const handleDragLeave = (e: React.DragEvent<HTMLDivElement>) => {
    const rect = e.currentTarget.getBoundingClientRect();
    const x = e.clientX;
    const y = e.clientY;
    
    if (x < rect.left || x > rect.right || y < rect.top || y > rect.bottom) {
      console.log('🎯 SIDEBAR - Drag Leave (outside bounds)');
      setIsOver(false);
      setCanDrop(false);
    }
  };

  const handleDrop = (e: React.DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    console.log('🎯 DROPPABLE SIDEBAR - Drop event triggered! Mode:', mode);
    setIsOver(false);
    setCanDrop(false);
    
    const planData = e.dataTransfer.getData('application/json');
    const planId = e.dataTransfer.getData('text/plain');
    
    console.log('📤 DROPPABLE SIDEBAR - Drop received:', {
      planId: planId,
      mode: mode,
      hasPlanData: !!planData,
      planDataLength: planData ? planData.length : 0
    });
    
    if (planData && planId) {
      try {
        const plan = JSON.parse(planData) as ProductionPlan;
        
        console.log('📋 DROPPABLE SIDEBAR - Plan parsed:', {
          id: plan.id,
          productName: plan.productName,
          status: plan.status,
          startTime: plan.startTime || 'UNSCHEDULED',
          endTime: plan.endTime || 'UNSCHEDULED',
          isScheduled: !!(plan.startTime && plan.endTime),
          mode: mode
        });
        
        console.log('✅ DROPPABLE SIDEBAR - Calling onDrop handler for:', plan.productName);
        onDrop(plan);
        
      } catch (error) {
        console.error('❌ DROPPABLE SIDEBAR - Parsing error:', error, 'Raw data:', planData);
        // Fallback - try to find the plan by ID in the current plans
        console.log('🔄 DROPPABLE SIDEBAR - Attempting fallback lookup for planId:', planId);
        toast.error('Error processing plan data', { 
          description: 'Could not parse drag data - trying fallback',
          duration: 2000 
        });
      }
    } else {
      console.log('❌ DROPPABLE SIDEBAR - Missing data:', {
        hasPlanData: !!planData,
        planId: planId,
        dataTransferItems: e.dataTransfer.items.length
      });
      toast.error('Missing plan data', { 
        description: 'Please try dragging again',
        duration: 2000 
      });
    }
  };

  return (
    <div 
      onDragEnter={handleDragEnter}
      onDragOver={handleDragOver}
      onDragLeave={handleDragLeave}
      onDrop={handleDrop}
      className={`h-full transition-all duration-200 relative rounded-lg ${
        isOver ? (canDrop ? 'bg-blue-50 border-blue-300 border-2 border-dashed' : 'bg-red-50 border-red-300 border-2 border-dashed') : ''
      }`}
    >
      {children}
      {isOver && canDrop && (
        <div className="absolute inset-0 flex items-center justify-center bg-blue-50/90 rounded-lg z-50 pointer-events-none">
          <div className="text-blue-700 font-medium text-center">
            <div className="text-lg mb-1">📤</div>
            <div>Drop to unschedule</div>
          </div>
        </div>
      )}
      {isOver && !canDrop && (
        <div className="absolute inset-0 flex items-center justify-center bg-red-50/90 rounded-lg z-50 pointer-events-none">
          <div className="text-red-700 font-medium text-center">
            <div className="text-lg mb-1">❌</div>
            <div>Cannot unschedule</div>
          </div>
        </div>
      )}
    </div>
  );
}

export function MachineCalendarView({ 
  machineId, 
  onBack, 
  onOpenInNewTab,
  mode = 'planning',
  breadcrumbContext = 'Planning > Machine Schedule'
}: MachineCalendarViewProps) {
  const [viewMode, setViewMode] = useState<'month' | 'week' | 'day'>('day');
  const [selectedDate, setSelectedDate] = useState(new Date());
  const [plans, setPlans] = useState<ProductionPlan[]>([]);
  const [selectedPlan, setSelectedPlan] = useState<ProductionPlan | null>(null);
  
  // Use job schedule context for cross-machine transfers and MCR management
  const { 
    transferJobToMachine, 
    updatePlan, 
    mcrRequests,
    getMCRsForMachine,
    checkMoldChange,
    createMCR 
  } = useJobSchedule();
  
  // Persistent storage for user modifications to plans - RESTORED
  const [planModifications, setPlanModifications] = useState<Record<string, {
    startTime?: string;
    endTime?: string;
    date?: Date;
    isUnscheduled?: boolean;
  }>>({});
  
  // Helper function to generate consistent plan keys - RESTORED
  const generatePlanKey = useCallback((plan: ProductionPlan) => {
    const originalDate = format(plan.date, 'yyyy-MM-dd');
    return `${plan.productId}-${plan.machineId}-${originalDate}`;
  }, []);
  
  // Production Control specific state
  const [rescheduleModalOpen, setRescheduleModalOpen] = useState(false);
  const [rescheduleData, setRescheduleData] = useState<{
    plan: ProductionPlan;
    fromTime: string;
    toTime: string;
    fromMachine: string;
    toMachine: string;
  } | null>(null);
  const [rescheduleReason, setRescheduleReason] = useState('');
  const [rescheduleNote, setRescheduleNote] = useState('');
  
  // Enhanced reschedule state for precise time/date control
  const [rescheduleDate, setRescheduleDate] = useState<Date>(new Date());
  const [rescheduleStartTime, setRescheduleStartTime] = useState('');
  const [rescheduleEndTime, setRescheduleEndTime] = useState('');
  const [showDatePicker, setShowDatePicker] = useState(false);
  
  // Machine transfer specific state
  const [transferModalOpen, setTransferModalOpen] = useState(false);
  const [transferData, setTransferData] = useState<{
    jobId: string;
    fromMachineId: string;
    toMachineId: string;
    productName: string;
    quantity: number;
  } | null>(null);
  const [transferReason, setTransferReason] = useState('');

  // Update plans to use selected date and machine - RESTORED with modifications applied
  useEffect(() => {
    console.log('Regenerating plans - selectedDate:', selectedDate, 'machineId:', machineId, 'modifications count:', Object.keys(planModifications).length);
    
    const allPlans: ProductionPlan[] = [];
    
    // Generate plans for current date and nearby dates for better calendar view
    const startDate = new Date(selectedDate);
    const endDate = new Date(selectedDate);
    startDate.setDate(startDate.getDate() - 7); // 7 days before
    endDate.setDate(endDate.getDate() + 14); // 14 days after
    
    for (let d = new Date(startDate); d <= endDate; d.setDate(d.getDate() + 1)) {
      const dayPlans = generateDateAwarePlans(machineId, new Date(d));
      allPlans.push(...dayPlans);
    }
    
    // Apply user modifications to the generated plans
    const modifiedPlans = allPlans.map(plan => {
      const planKey = generatePlanKey(plan);
      const modification = planModifications[planKey];
      
      if (modification) {
        const modifiedPlan = {
          ...plan,
          startTime: modification.startTime !== undefined ? modification.startTime : plan.startTime,
          endTime: modification.endTime !== undefined ? modification.endTime : plan.endTime,
          date: modification.date || plan.date
        };
        return modifiedPlan;
      }
      
      return plan;
    });
    
    setPlans(modifiedPlans);
  }, [selectedDate, machineId, planModifications, generatePlanKey, mode]);



  // Get the machine data based on the machineId prop
  const currentMachine = mockMachines[machineId] || mockMachines['IM-001']; // fallback to IM-001 if not found

  const getStatusColor = (status: string) => {
    if (mode === 'production-control') {
      switch (status) {
        case 'draft':
          return 'bg-blue-500'; // Draft Plan = BLUE (#3B82F6)
        case 'final':
          return 'bg-yellow-400'; // Final Plan = YELLOW (#FACC15)
        case 'approved':
          return 'bg-orange-500'; // Approved Job ID = ORANGE (#F97316)
        case 'running':
          return 'bg-green-500'; // Running = GREEN (#22C55E)
        case 'paused':
          return 'bg-red-500'; // Paused/Downtime = RED (#EF4444)
        case 'completed':
          return 'bg-green-800'; // Completed = DARK GREEN (#166534)
        default:
          return 'bg-slate-500';
      }
    } else {
      // Planning mode colors - Following exact color scheme
      switch (status) {
        case 'draft':
          return 'bg-blue-500'; // Draft Plan = BLUE (#3B82F6)
        case 'final':
          return 'bg-yellow-400'; // Final Plan = YELLOW (#FACC15)
        case 'approved':
          return 'bg-orange-500'; // Approved Plan with Job ID = ORANGE (#F97316)
        case 'running':
          return 'bg-green-500'; // Running Job = GREEN (#22C55E)
        case 'completed':
          return 'bg-green-800'; // Completed Job = DARK GREEN (#166534)
        default:
          return 'bg-slate-500';
      }
    }
  };

  const getStatusBorderColor = (status: string) => {
    if (mode === 'production-control') {
      switch (status) {
        case 'draft':
          return 'border-blue-500'; // BLUE (#3B82F6)
        case 'final':
          return 'border-yellow-400'; // YELLOW (#FACC15)
        case 'approved':
          return 'border-orange-500'; // ORANGE (#F97316)
        case 'running':
          return 'border-green-500'; // GREEN (#22C55E)
        case 'paused':
          return 'border-red-500'; // RED (#EF4444)
        case 'completed':
          return 'border-green-800'; // DARK GREEN (#166534)
        default:
          return 'border-slate-500';
      }
    } else {
      // Planning mode border colors - Following exact color scheme
      switch (status) {
        case 'draft':
          return 'border-blue-500'; // Draft Plan = BLUE (#3B82F6)
        case 'final':
          return 'border-yellow-400'; // Final Plan = YELLOW (#FACC15)
        case 'approved':
          return 'border-orange-500'; // Approved Plan with Job ID = ORANGE (#F97316)
        case 'running':
          return 'border-green-500'; // Running Job = GREEN (#22C55E)
        case 'completed':
          return 'border-green-800'; // Completed Job = DARK GREEN (#166534)
        default:
          return 'border-slate-500';
      }
    }
  };

  const getStatusTextColor = (status: string) => {
    if (mode === 'production-control') {
      switch (status) {
        case 'draft':
          return 'text-white'; // White text on blue background
        case 'final':
          return 'text-slate-900'; // Dark text on yellow background
        case 'approved':
          return 'text-white'; // White text on orange background
        case 'running':
          return 'text-white'; // White text on green background
        case 'paused':
          return 'text-white'; // White text on red background
        case 'completed':
          return 'text-white'; // White text on dark green background
        default:
          return 'text-slate-700';
      }
    } else {
      // Planning mode text colors - Following exact color scheme
      switch (status) {
        case 'draft':
          return 'text-white'; // White text on blue background
        case 'final':
          return 'text-slate-900'; // Dark text on yellow background
        case 'approved':
          return 'text-white'; // White text on orange background
        case 'running':
          return 'text-white'; // White text on green background
        case 'completed':
          return 'text-white'; // White text on dark green background
        default:
          return 'text-white';
      }
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'draft':
        return '📋'; // Draft plan
      case 'final':
        return '⏳'; // Waiting approval
      case 'approved':
        return '🚀'; // Ready to run 
      case 'running':
        return '⚡'; // Running
      case 'paused':
        return '⏸️'; // Paused
      case 'completed':
        return '✅'; // Completed
      default:
        return '📋';
    }
  };

  const navigateDate = (direction: 'prev' | 'next') => {
    if (viewMode === 'month') {
      setSelectedDate(direction === 'next' ? addMonths(selectedDate, 1) : subMonths(selectedDate, 1));
    } else if (viewMode === 'week') {
      setSelectedDate(direction === 'next' ? addWeeks(selectedDate, 1) : subWeeks(selectedDate, 1));
    } else {
      setSelectedDate(direction === 'next' ? addDays(selectedDate, 1) : addDays(selectedDate, -1));
    }
  };

  const getPlansForDate = (date: Date) => {
    return plans.filter(plan => isSameDay(plan.date, date));
  };

  const getScheduledPlans = (date: Date) => {
    return getPlansForDate(date).filter(plan => plan.startTime && plan.endTime);
  };

  const getUnscheduledPlans = () => {
    // Get unscheduled plans for the current machine, filtering by mode
    let unscheduledPlans;
    
    if (mode === 'production-control') {
      // In production control, only approved jobs without schedule are draggable
      unscheduledPlans = plans.filter(plan => 
        plan.status === 'approved' && 
        plan.jobId && 
        (!plan.startTime || !plan.endTime) && 
        plan.machineId === machineId
      );
    } else {
      // In planning mode, draft and final plans without schedule are draggable
      unscheduledPlans = plans.filter(plan => 
        (plan.status === 'draft' || plan.status === 'final') && 
        (!plan.startTime || !plan.endTime) && 
        plan.machineId === machineId
      );
    }
    
    console.log(`📋 UNSCHEDULED PLANS for ${mode} mode:`, {
      totalPlans: plans.length,
      machineId: machineId,
      unscheduledCount: unscheduledPlans.length,
      scheduledCount: plans.filter(p => p.startTime && p.endTime && p.machineId === machineId).length,
      allPlanStatuses: plans.filter(p => p.machineId === machineId).map(p => ({
        id: p.id,
        status: p.status,
        isScheduled: !!(p.startTime && p.endTime)
      })),
      unscheduledDetails: unscheduledPlans.map(p => ({
        id: p.id,
        productName: p.productName,
        status: p.status,
        startTime: p.startTime,
        endTime: p.endTime,
        jobId: p.jobId
      }))
    });
    
    return unscheduledPlans;
  };

  // Get all plans of a specific status across all dates (for Production Control mode sidebar)
  const getAllPlansByStatus = (status: string) => {
    return plans.filter(plan => plan.status === status);
  };

  // Get all unscheduled approved jobs (approved status but no start time) across all dates
  const getAllUnscheduledApprovedJobs = () => {
    return plans.filter(plan => 
      plan.status === 'approved' && 
      plan.jobId && 
      (!plan.startTime || !plan.endTime) &&
      plan.machineId === machineId
    );
  };

  // Handle plan drop on timeline - RESTORED WORKING VERSION
  const handlePlanDrop = useCallback((plan: ProductionPlan, hour: number, date?: Date, isFromAnotherMachine = false) => {
    const dropDate = date || selectedDate;
    
    console.log('🎯 TIMELINE DROP - Plan:', plan.id, 'Hour:', hour, 'Status:', plan.status);
    
    // Skip cross-machine transfers for now
    if (isFromAnotherMachine) {
      return;
    }
    
    // Calculate exact times for the drop
    const newStartTime = `${hour.toString().padStart(2, '0')}:00`;
    const endHour = Math.min(hour + 4, 23); // 4-hour duration, max 23:00
    const newEndTime = `${endHour.toString().padStart(2, '0')}:00`;
    
    if (mode === 'production-control' && plan.jobId) {
      // Production control mode - show confirmation
      setRescheduleData({
        plan,
        fromTime: plan.startTime || 'Unscheduled',
        toTime: newStartTime,
        fromMachine: currentMachine.name,
        toMachine: currentMachine.name
      });
      setRescheduleDate(dropDate);
      setRescheduleStartTime(newStartTime);
      setRescheduleEndTime(newEndTime);
      setRescheduleModalOpen(true);
    } else {
      // PLANNING MODE - update with planModifications
      const wasUnscheduled = !plan.startTime || !plan.endTime;
      
      // Validate plan can be scheduled in planning mode
      if (plan.status !== 'draft' && plan.status !== 'final') {
        toast.error('Cannot schedule plan', {
          description: `Only draft and final plans can be scheduled (current: ${plan.status})`,
          duration: 3000,
        });
        return;
      }
      
      const planKey = generatePlanKey(plan);
      
      // Update modifications
      setPlanModifications(prev => ({
        ...prev,
        [planKey]: {
          startTime: newStartTime,
          endTime: newEndTime,
          date: dropDate,
          isUnscheduled: false
        }
      }));
      
      // Success notification
      const message = wasUnscheduled ? 'Plan scheduled' : 'Plan rescheduled';
      toast.success(message, {
        description: `${plan.productName} scheduled at ${newStartTime}-${newEndTime}`,
        duration: 2000,
      });
    }
  }, [selectedDate, mode, currentMachine.name, generatePlanKey]);

  // Handle unscheduling a plan (drag to sidebar)
  const handleUnschedule = useCallback((planId: string, plan?: ProductionPlan) => {
    console.log('Unscheduling plan:', planId, 'Plan status:', plan?.status);
    
    if (plan) {
      // Store modification persistently
      const planKey = generatePlanKey(plan);
      setPlanModifications(prev => ({
        ...prev,
        [planKey]: {
          startTime: '',
          endTime: '',
          date: plan.date,
          isUnscheduled: true
        }
      }));
    }
    
    // Update plans state to remove schedule
    setPlans(prevPlans => {
      const updatedPlans = prevPlans.map(p => {
        if (p.id === planId) {
          console.log(`Unscheduling plan ${p.id}: removing times ${p.startTime}-${p.endTime}`);
          return { ...p, startTime: '', endTime: '' };
        }
        return p;
      });
      
      console.log('After unscheduling - Unscheduled plans:', updatedPlans.filter(p => !p.startTime || !p.endTime).length);
      return updatedPlans;
    });
    
    console.log(`Successfully unscheduled plan ${planId}`);
  }, [generatePlanKey]);

  // Handle sidebar drop for unscheduling - ENHANCED DEBUGGING VERSION
  const handleSidebarDrop = useCallback((plan: ProductionPlan) => {
    console.log('📤 SIDEBAR DROP - Plan details:', {
      id: plan.id,
      productName: plan.productName,
      status: plan.status,
      startTime: plan.startTime,
      endTime: plan.endTime,
      jobId: plan.jobId,
      mode: mode,
      isScheduled: !!(plan.startTime && plan.endTime),
      machineId: plan.machineId,
      currentMachineId: machineId
    });
    
    // Validation
    let canUnschedule = false;
    let validationError = '';
    
    if (mode === 'planning') {
      console.log('📤 PLANNING MODE validation for:', plan.productName);
      if (plan.status === 'draft' || plan.status === 'final') {
        if (plan.startTime && plan.endTime) {
          canUnschedule = true;
          console.log('✅ Planning validation passed - can unschedule');
        } else {
          validationError = 'Plan is not currently scheduled';
          console.log('❌ Planning validation failed - not scheduled');
        }
      } else {
        validationError = `Only draft/final plans can be unscheduled (current: ${plan.status})`;
        console.log('❌ Planning validation failed - wrong status');
      }
    } else {
      console.log('📤 PRODUCTION CONTROL validation for:', plan.productName);
      if (plan.status === 'approved' && plan.jobId) {
        if (plan.startTime && plan.endTime) {
          canUnschedule = true;
          console.log('✅ Production validation passed - can unschedule');
        } else {
          validationError = 'Job is not currently scheduled';
          console.log('❌ Production validation failed - not scheduled');
        }
      } else {
        validationError = `Only approved jobs can be unscheduled (current: ${plan.status})`;
        console.log('❌ Production validation failed - wrong status or no job ID');
      }
    }
    
    if (!canUnschedule) {
      console.log('❌ UNSCHEDULE REJECTED:', validationError);
      toast.error('Cannot unschedule', {
        description: validationError,
        duration: 3000,
      });
      return;
    }
    
    console.log('✅ UNSCHEDULE APPROVED - proceeding with modifications');
    
    const planKey = generatePlanKey(plan);
    console.log('📝 Generated plan key:', planKey);
    
    // Update modifications to unschedule
    setPlanModifications(prev => {
      const updated = {
        ...prev,
        [planKey]: {
          startTime: '',
          endTime: '',
          date: plan.date,
          isUnscheduled: true
        }
      };
      console.log('📝 Updated plan modifications:', updated);
      console.log('📊 All plan modifications after update:', Object.keys(updated).length);
      return updated;
    });
    
    // Also force immediate UI update by updating plans state
    console.log('🔄 Force updating plans state for immediate UI feedback');
    setPlans(prevPlans => {
      const updatedPlans = prevPlans.map(p => {
        if (p.id === plan.id) {
          console.log(`⚡ Immediately unscheduling plan ${p.id} in state`);
          return { ...p, startTime: '', endTime: '' };
        }
        return p;
      });
      return updatedPlans;
    });
    
    // Success notification
    const itemType = mode === 'production-control' ? 'Job' : 'Plan';
    toast.success(`${itemType} unscheduled successfully`, {
      description: `${plan.productName} moved to unscheduled plans`,
      duration: 2000,
    });
    
    console.log('✅ SIDEBAR DROP COMPLETED for:', plan.productName);
  }, [mode, generatePlanKey]);

  // Handle reschedule confirmation
  const handleRescheduleConfirm = useCallback(() => {
    if (!rescheduleData || !rescheduleReason || !rescheduleStartTime || !rescheduleEndTime) return;
    
    // Validate that end time is after start time
    const startHour = parseInt(rescheduleStartTime.split(':')[0]);
    const startMinute = parseInt(rescheduleStartTime.split(':')[1]);
    const endHour = parseInt(rescheduleEndTime.split(':')[0]);
    const endMinute = parseInt(rescheduleEndTime.split(':')[1]);
    
    const startTotalMinutes = startHour * 60 + startMinute;
    const endTotalMinutes = endHour * 60 + endMinute;
    
    if (endTotalMinutes <= startTotalMinutes) {
      toast.error('End time must be after start time');
      return;
    }
    
    const wasUnscheduled = !rescheduleData.plan.startTime || !rescheduleData.plan.endTime;
    
    // Store modification persistently
    const planKey = generatePlanKey(rescheduleData.plan);
    setPlanModifications(prev => ({
      ...prev,
      [planKey]: {
        startTime: rescheduleStartTime,
        endTime: rescheduleEndTime,
        date: rescheduleDate,
        isUnscheduled: false
      }
    }));
    
    // Update plans state with consistent state management
    setPlans(prevPlans => {
      const updatedPlans = prevPlans.map(p => {
        if (p.id === rescheduleData.plan.id) {
          console.log(`Rescheduling plan ${p.id} to ${rescheduleStartTime}-${rescheduleEndTime}`);
          return { ...p, startTime: rescheduleStartTime, endTime: rescheduleEndTime, date: rescheduleDate };
        }
        return p;
      });
      
      console.log('After reschedule - Unscheduled jobs:', updatedPlans.filter(p => !p.startTime || !p.endTime).length);
      return updatedPlans;
    });
    
    // Log the reschedule action
    console.log('Job rescheduled:', {
      jobId: rescheduleData.plan.jobId,
      from: rescheduleData.fromTime,
      to: `${rescheduleStartTime} - ${rescheduleEndTime}`,
      date: format(rescheduleDate, 'yyyy-MM-dd'),
      reason: rescheduleReason,
      note: rescheduleNote,
      timestamp: new Date().toISOString(),
      wasUnscheduled
    });
    
    // Show success toast
    toast.success(`Job ${rescheduleData.plan.jobId} ${wasUnscheduled ? 'scheduled' : 'rescheduled'} to ${format(rescheduleDate, 'MMM d')} at ${rescheduleStartTime}-${rescheduleEndTime}`);
    
    // Reset modal state
    setRescheduleModalOpen(false);
    setRescheduleData(null);
    setRescheduleReason('');
    setRescheduleNote('');
    setRescheduleDate(new Date());
    setRescheduleStartTime('');
    setRescheduleEndTime('');
  }, [rescheduleData, rescheduleReason, rescheduleNote, rescheduleStartTime, rescheduleEndTime, rescheduleDate, generatePlanKey]);

  // Handle machine transfer confirmation
  const handleTransferConfirm = useCallback(() => {
    if (!transferData || !transferReason) return;
    
    const success = transferJobToMachine(
      transferData.jobId,
      transferData.fromMachineId,
      transferData.toMachineId
    );
    
    if (success) {
      console.log('Job transferred:', {
        jobId: transferData.jobId,
        from: transferData.fromMachineId,
        to: transferData.toMachineId,
        reason: transferReason
      });
      
      // Refresh the current machine's data
      const allPlans: ProductionPlan[] = [];
      const startDate = new Date(selectedDate);
      const endDate = new Date(selectedDate);
      startDate.setDate(startDate.getDate() - 7);
      endDate.setDate(endDate.getDate() + 14);
      
      for (let d = new Date(startDate); d <= endDate; d.setDate(d.getDate() + 1)) {
        const dayPlans = generateDateAwarePlans(machineId, new Date(d));
        allPlans.push(...dayPlans);
      }
      setPlans(allPlans);
    }
    
    setTransferModalOpen(false);
    setTransferData(null);
    setTransferReason('');
  }, [transferData, transferReason, transferJobToMachine, selectedDate, machineId]);

  // Handle quick machine transfer from context menu
  const handleQuickTransfer = useCallback((plan: ProductionPlan, targetMachineId: string) => {
    if (!plan.jobId) return;

    // Remove job from current machine
    setPlans(prevPlans => prevPlans.filter(p => p.id !== plan.id));

    // Show success message
    const targetMachineName = mockMachines[targetMachineId]?.name || targetMachineId;
    toast.success(`Job ${plan.jobId} transferred to ${targetMachineName}`, {
      description: `${plan.productName} (${plan.quantity.toLocaleString()} pcs)`,
      duration: 4000,
    });

    console.log('Quick transfer completed:', {
      jobId: plan.jobId,
      productName: plan.productName,
      from: machineId,
      to: targetMachineId,
      reason: 'Quick transfer from Job Queue'
    });
  }, [machineId]);

  // Handle plan transfer for Draft and Final plans
  const handlePlanTransfer = useCallback((plan: ProductionPlan, targetMachineId: string) => {
    if (plan.status !== 'draft' && plan.status !== 'final') return;

    // Remove plan from current machine
    setPlans(prevPlans => prevPlans.filter(p => p.id !== plan.id));

    // Show success message
    const targetMachineName = mockMachines[targetMachineId]?.name || targetMachineId;
    const planTypeText = plan.status === 'draft' ? 'Draft Plan' : 'Final Plan';
    toast.success(`${planTypeText} transferred to ${targetMachineName}`, {
      description: `${plan.productName} (${plan.quantity.toLocaleString()} pcs) • Plan ID: ${plan.id}`,
      duration: 4000,
    });

    console.log('Plan transfer completed:', {
      planId: plan.id,
      status: plan.status,
      productName: plan.productName,
      from: machineId,
      to: targetMachineId,
      reason: 'Cross-machine plan transfer'
    });
  }, [machineId]);

  const renderDayViewTimeline = () => {
    const hours = Array.from({ length: 24 }, (_, i) => i);
    const scheduledPlans = getScheduledPlans(selectedDate);

    return (
      <div className="flex h-full">
        {/* Timeline Container */}
        <div className="flex-1 relative">
          {/* Hour Grid Background */}
          <div className="absolute inset-0">
            {hours.map((hour) => (
              <DroppableTimeSlot key={hour} hour={hour} date={selectedDate} onDrop={handlePlanDrop} mode={mode}>
                <div className="h-16 border-b border-slate-200 flex items-center px-4 bg-slate-50">
                  <span className="text-sm text-slate-600 font-medium w-16">
                    {hour.toString().padStart(2, '0')}:00
                  </span>
                </div>
              </DroppableTimeSlot>
            ))}
          </div>

          {/* Scheduled Plans Overlay */}
          <div className="absolute inset-0">
            {scheduledPlans.map((plan) => {
              if (!plan.startTime || !plan.endTime) return null;

              const startHour = parseInt(plan.startTime.split(':')[0]);
              const startMinute = parseInt(plan.startTime.split(':')[1]);
              const endHour = parseInt(plan.endTime.split(':')[0]);
              const endMinute = parseInt(plan.endTime.split(':')[1]);

              const startPosition = startHour * 64 + (startMinute / 60) * 64; // 64px per hour
              const duration = (endHour - startHour) + (endMinute - startMinute) / 60;
              const height = duration * 64; // 64px per hour

              return (
                <div
                  key={plan.id}
                  className="absolute left-20 right-4"
                  style={{
                    top: `${startPosition}px`,
                    height: `${height}px`,
                    minHeight: '60px'
                  }}
                >
                  <DraggablePlan plan={plan} mode={mode}>
                    <div
                      className={`w-full h-full ${getStatusColor(plan.status)} ${getStatusTextColor(plan.status)} rounded-lg shadow-sm border-2 ${getStatusBorderColor(plan.status)} hover:shadow-md transition-shadow ${
                        (mode === 'production-control' && plan.status === 'approved' && plan.jobId) || 
                        (mode === 'planning' && (plan.status === 'draft' || plan.status === 'final')) 
                          ? 'cursor-move' 
                          : 'cursor-pointer'
                      }`}
                    >
                    <TooltipProvider>
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <div className="p-3 h-full flex flex-col justify-between">
                            <div className="space-y-1">
                              <div className="flex items-center justify-between">
                                <div className="flex items-center gap-2">
                                  {mode === 'production-control' && plan.status === 'approved' && (
                                    <GripVertical className="h-4 w-4 opacity-70" />
                                  )}
                                  {mode === 'planning' && (plan.status === 'draft' || plan.status === 'final') && (
                                    <GripVertical className="h-4 w-4 opacity-70" />
                                  )}
                                  <div className="flex-1">
                                    {mode === 'production-control' && plan.jobId && (
                                      <div className="font-bold text-sm mb-1">{plan.jobId}</div>
                                    )}
                                    <div className="text-xs opacity-80 mb-1">Plan ID: {plan.id}</div>
                                     <h4 className="font-semibold truncate">{plan.productName}</h4>
                                  </div>
                                </div>
                                {plan.status === 'completed' && (
                                  <CheckCircle2 className="h-4 w-4" />
                                )}
                              </div>
                              <p className="text-sm opacity-90">{plan.quantity.toLocaleString()} pcs</p>
                              <p className="text-sm opacity-75">{plan.startTime} - {plan.endTime}</p>
                              {plan.operatorName && (
                                <div className="flex items-center gap-1 text-sm opacity-80">
                                  <User className="h-3 w-3" />
                                  <span>{plan.operatorName}</span>
                                </div>
                              )}
                            </div>
                            <div className="flex items-center justify-between">
                              <Badge 
                                variant="secondary" 
                                className="bg-white/20 text-white border-white/30 text-xs"
                              >
                                {getStatusIcon(plan.status)} {plan.status.charAt(0).toUpperCase() + plan.status.slice(1)}
                              </Badge>
                              {mode === 'production-control' && plan.status === 'approved' && (
                                <span className="text-xs opacity-70">🚀 Drag to reschedule or sidebar to unschedule</span>
                              )}
                              {mode === 'planning' && (plan.status === 'draft' || plan.status === 'final') && (
                                <span className="text-xs opacity-70">📱 Drag to reschedule or sidebar to unschedule</span>
                              )}
                            </div>
                          </div>
                        </TooltipTrigger>
                        <TooltipContent>
                          <div className="space-y-2 max-w-xs">
                            <div className="flex items-center gap-2">
                              <ImageWithFallback
                                src={plan.productImage}
                                alt={plan.productName}
                                className="w-10 h-10 rounded object-cover"
                              />
                              <div>
                                <p className="font-semibold">{plan.productName}</p>
                                <p className="text-sm text-slate-600">Plan ID: {plan.id}</p>
                                {plan.jobId && <p className="text-sm text-slate-600">Job ID: {plan.jobId}</p>}
                              </div>
                            </div>
                            <div className="space-y-1 text-sm">
                              <p>Quantity: {plan.quantity.toLocaleString()} pcs</p>
                              <p>Duration: {plan.startTime} - {plan.endTime}</p>
                              <p>Status: {plan.status}</p>
                              {plan.operatorName && <p>Operator: {plan.operatorName}</p>}
                              {plan.plannerName && <p>Planner: {plan.plannerName}</p>}
                            </div>
                            {plan.materialRequirements && (
                              <div className="border-t pt-2">
                                <p className="text-sm font-medium">Materials Required:</p>
                                <ul className="text-sm text-slate-600 space-y-1">
                                  <li>Glue: {plan.materialRequirements.glue} kg</li>
                                  <li>Color: {plan.materialRequirements.color}</li>
                                  <li>Bags: {plan.materialRequirements.bags}</li>
                                </ul>
                              </div>
                            )}
                          </div>
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>
                    </div>
                  </DraggablePlan>
                </div>
              );
            })}
          </div>
        </div>
      </div>
    );
  };

  const renderMonthView = () => {
    const monthStart = startOfMonth(selectedDate);
    const monthEnd = endOfMonth(selectedDate);
    const weeks = eachWeekOfInterval({ start: monthStart, end: monthEnd });

    return (
      <div className="space-y-4">
        {/* Month Header */}
        <div className="grid grid-cols-7 gap-2 mb-4">
          {['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map((day) => (
            <div key={day} className="p-3 text-center font-semibold text-slate-600 bg-slate-100 rounded">
              {day}
            </div>
          ))}
        </div>

        {/* Month Calendar */}
        <div className="space-y-2">
          {weeks.map((weekStart, weekIndex) => {
            const days = eachDayOfInterval({ start: weekStart, end: endOfWeek(weekStart) });
            
            return (
              <div key={weekIndex} className="grid grid-cols-7 gap-2">
                {days.map((day) => {
                  const dayPlans = getPlansForDate(day);
                  const scheduledPlans = getScheduledPlans(day);
                  const isToday = isSameDay(day, new Date());
                  const isSelected = isSameDay(day, selectedDate);
                  
                  return (
                    <Card 
                      key={day.toISOString()} 
                      className={`min-h-[120px] cursor-pointer transition-all duration-200 hover:shadow-md ${isToday ? 'ring-2 ring-blue-500' : ''} ${isSelected ? 'bg-blue-50 border-blue-300' : ''}`}
                      onClick={() => {
                        setSelectedDate(day);
                        setViewMode('day');
                      }}
                    >
                      <CardHeader className="p-2">
                        <div className="flex justify-between items-center">
                          <span className={`font-semibold ${isToday ? 'text-blue-600' : 'text-slate-900'}`}>
                            {format(day, 'd')}
                          </span>
                          {dayPlans.length > 0 && (
                            <div className="flex items-center gap-1">
                              <Badge variant="secondary" className="text-xs">
                                {dayPlans.length}
                              </Badge>
                              {scheduledPlans.length !== dayPlans.length && (
                                <div className="w-2 h-2 bg-orange-500 rounded-full" title="Has unscheduled plans"></div>
                              )}
                            </div>
                          )}
                        </div>
                      </CardHeader>
                      <CardContent className="p-2 pt-0">
                        <div className="space-y-1">
                          {scheduledPlans.slice(0, 2).map((plan) => (
                            <div key={plan.id} className={`p-1 rounded text-xs ${getStatusTextColor(plan.status)} ${getStatusColor(plan.status)}`}>
                              <div className="flex items-center gap-1">
                                <span>{getStatusIcon(plan.status)}</span>
                                <span className="truncate">{plan.productName.split(' ').slice(0, 2).join(' ')}</span>
                              </div>
                              <div className="text-xs opacity-90">
                                {plan.startTime} - {plan.endTime}
                              </div>
                            </div>
                          ))}
                          {scheduledPlans.length > 2 && (
                            <div className="text-xs text-slate-600 text-center py-1">
                              +{scheduledPlans.length - 2} more
                            </div>
                          )}
                          {scheduledPlans.length === 0 && dayPlans.length > 0 && (
                            <div className="text-xs text-orange-600 text-center py-1 font-medium">
                              {dayPlans.length} unscheduled
                            </div>
                          )}
                        </div>
                      </CardContent>
                    </Card>
                  );
                })}
              </div>
            );
          })}
        </div>
      </div>
    );
  };

  const renderWeekView = () => {
    const weekStart = startOfWeek(selectedDate);
    const days = eachDayOfInterval({ start: weekStart, end: endOfWeek(weekStart) });
    const hours = Array.from({ length: 24 }, (_, i) => i);

    return (
      <div className="space-y-4">
        {/* Week Header */}
        <div className="grid grid-cols-8 gap-2">
          <div className="p-2">
            {mode === 'production-control' && (
              <div className="text-xs text-slate-500 flex items-center gap-1">
                <GripVertical className="h-3 w-3" />
                <span>Drag 🟠 Jobs</span>
              </div>
            )}
          </div>
          {days.map((day) => (
            <div key={day.toISOString()} className="p-3 text-center">
              <div className="font-semibold text-slate-900">{format(day, 'EEE')}</div>
              <div className={`text-lg ${isSameDay(day, new Date()) ? 'text-blue-600 font-bold' : 'text-slate-600'}`}>
                {format(day, 'd')}
              </div>
            </div>
          ))}
        </div>

        {/* Week Timeline */}
        <div className="grid grid-cols-8 gap-2">
          {/* Time Column */}
          <div className="space-y-2">
            {hours.map((hour) => (
              <div key={hour} className="h-16 flex items-center justify-center text-sm text-slate-600 border-t">
                {hour.toString().padStart(2, '0')}:00
              </div>
            ))}
          </div>

          {/* Day Columns */}
          {days.map((day) => (
            <div key={day.toISOString()} className="relative">
              {/* Individual hour drop zones */}
              <div className="space-y-2">
                {hours.map((hour) => (
                  <DroppableTimeSlot 
                    key={hour}
                    hour={hour} 
                    date={day} 
                    onDrop={(plan, hour, date) => handlePlanDrop(plan, hour, date)} 
                    mode={mode}
                  >
                    <div className="h-16 border border-slate-200 rounded bg-slate-50"></div>
                  </DroppableTimeSlot>
                ))}
              </div>
              
              {/* Continuous plan bars overlay */}
              <div className="absolute inset-0 pointer-events-none">
                {getScheduledPlans(day).map((plan) => {
                  if (!plan.startTime || !plan.endTime) return null;
                  
                  const startHour = parseInt(plan.startTime.split(':')[0]);
                  const startMinute = parseInt(plan.startTime.split(':')[1]);
                  const endHour = parseInt(plan.endTime.split(':')[0]);
                  const endMinute = parseInt(plan.endTime.split(':')[1]);
                  
                  // Calculate position and height
                  const startPosition = (startHour * 72) + (startMinute / 60 * 72); // 72px = 16*4.5 (h-16 + gap)
                  const endPosition = (endHour * 72) + (endMinute / 60 * 72);
                  const height = endPosition - startPosition;
                  
                  // Check if this plan is draggable based on mode
                  const isDraggableInWeek = mode === 'production-control' 
                    ? plan.status === 'approved' && plan.jobId  // Production control: only approved jobs with Job IDs
                    : (plan.status === 'draft' || plan.status === 'final'); // Planning mode: draft and final plans
                  
                  const planBlock = (
                    <div 
                      className={`absolute left-1 right-1 rounded p-2 text-xs ${getStatusTextColor(plan.status)} ${getStatusColor(plan.status)} border-2 ${getStatusBorderColor(plan.status)} hover:shadow-md transition-shadow ${
                        isDraggableInWeek ? 'cursor-move' : 'cursor-pointer'
                      }`}
                      style={{
                        top: `${startPosition}px`,
                        height: `${height}px`,
                        minHeight: '32px',
                        pointerEvents: 'auto'
                      }}
                      onClick={() => {
                        if (!isDraggableInWeek) {
                          setSelectedDate(day);
                          setViewMode('day');
                        }
                      }}
                    >
                      <div className="truncate font-medium">
                        {isDraggableInWeek && <GripVertical className="h-3 w-3 inline mr-1" />}
                        {plan.productName}
                      </div>
                      <div className="opacity-90 text-xs">{plan.quantity.toLocaleString()} pcs</div>
                      <div className="opacity-75 text-xs">{plan.startTime} - {plan.endTime}</div>
                      <div className="opacity-80 text-xs">Plan ID: {plan.id}</div>
                      {plan.jobId && (
                        <div className="opacity-80 text-xs font-medium">Job ID: {plan.jobId}</div>
                      )}
                      {plan.operatorName && (
                        <div className="opacity-80 text-xs truncate">{plan.operatorName}</div>
                      )}
                      {isDraggableInWeek && (
                        <div className="flex items-center justify-between">
                          <div className="opacity-90 text-xs font-medium">{plan.jobId}</div>
                          <div className="text-xs opacity-75">📤</div>
                        </div>
                      )}
                    </div>
                  );
                  
                  return (
                    <TooltipProvider key={plan.id}>
                      <Tooltip>
                        <TooltipTrigger asChild>
                          {isDraggableInWeek ? (
                            <DraggablePlan plan={plan} mode={mode}>
                              {planBlock}
                            </DraggablePlan>
                          ) : (
                            planBlock
                          )}
                        </TooltipTrigger>
                        <TooltipContent>
                          <div className="space-y-1">
                            <p className="font-semibold">{plan.productName}</p>
                            <p>Plan ID: {plan.id}</p>
                            {plan.jobId && <p>Job ID: {plan.jobId}</p>}
                            <p>Qty: {plan.quantity.toLocaleString()}</p>
                            <p>Time: {plan.startTime} - {plan.endTime}</p>
                            <p>Status: {plan.status}</p>
                            {plan.operatorName && <p>Operator: {plan.operatorName}</p>}
                            {isDraggableInWeek && (
                              <div className="space-y-1">
                                <p className="text-green-600 font-medium">✓ Draggable to other days</p>
                                <p className="text-blue-600 font-medium">📤 Transfer to compatible machines</p>
                                <p className="text-xs text-slate-500">
                                  Compatible with: {getCompatibleMachines(plan.machineId).map(id => mockMachines[id]?.name).join(', ')}
                                </p>
                              </div>
                            )}
                          </div>
                        </TooltipContent>
                      </Tooltip>
                    </TooltipProvider>
                  );
                })}
              </div>
            </div>
          ))}
        </div>
        
        {/* Week View Legend */}
        {mode === 'production-control' ? (
          <div className="mt-4 p-3 bg-slate-50 rounded-lg">
            <div className="text-xs font-medium text-slate-700 mb-2">Job Status Legend:</div>
            <div className="grid grid-cols-2 md:grid-cols-3 gap-2 text-xs">
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-blue-500 rounded"></div>
                <span>Draft Plan (View Only)</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-yellow-400 rounded"></div>
                <span>Final Plan (View Only)</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-orange-500 rounded"></div>
                <span>Approved Job (Draggable)</span>
                <GripVertical className="h-3 w-3 text-orange-500" />
              </div>
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-green-500 rounded"></div>
                <span>Running</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-red-500 rounded"></div>
                <span>Paused/Downtime</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-green-800 rounded"></div>
                <span>Completed</span>
              </div>
            </div>
          </div>
        ) : (
          <div className="mt-4 p-3 bg-slate-50 rounded-lg">
            <div className="text-xs font-medium text-slate-700 mb-2">Plan Status Legend:</div>
            <div className="grid grid-cols-2 md:grid-cols-3 gap-2 text-xs">
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-blue-500 rounded"></div>
                <span>📋 Draft Plan</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-yellow-400 rounded"></div>
                <span>⏳ Final Plan</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-orange-500 rounded"></div>
                <span>🚀 Approved Plan (Job ID)</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-green-500 rounded"></div>
                <span>⚡ Running Job</span>
              </div>
              <div className="flex items-center gap-2">
                <div className="w-3 h-3 bg-green-800 rounded"></div>
                <span>✅ Completed Job</span>
              </div>
            </div>
          </div>
        )}
      </div>
    );
  };

  return (
    <div className="flex h-screen bg-slate-50">
      {/* Main Content */}
      <div className="flex-1 flex flex-col">
        {/* Top Bar */}
        <div className="bg-white border-b border-slate-200 p-4">
          <div className="flex items-center justify-between">
            {/* Left Side */}
            <div className="flex items-center gap-4">
              <Button variant="ghost" size="sm" onClick={onBack}>
                <ArrowLeft className="h-4 w-4 mr-2" />
                Back
              </Button>
              
              <Breadcrumb>
                <BreadcrumbList>
                  {breadcrumbContext.split(' > ').map((part, index, array) => (
                    <React.Fragment key={index}>
                      <BreadcrumbItem>
                        {index === array.length - 1 ? (
                          <BreadcrumbPage>{part}</BreadcrumbPage>
                        ) : (
                          <BreadcrumbLink>{part}</BreadcrumbLink>
                        )}
                      </BreadcrumbItem>
                      {index < array.length - 1 && <BreadcrumbSeparator />}
                    </React.Fragment>
                  ))}
                  <BreadcrumbSeparator />
                  <BreadcrumbItem>
                    <BreadcrumbPage>{currentMachine.name}</BreadcrumbPage>
                  </BreadcrumbItem>
                </BreadcrumbList>
              </Breadcrumb>
            </div>

            {/* Right Side */}
            <div className="flex items-center gap-2">
              <div className="flex items-center bg-slate-100 rounded-lg p-1">
                <Button
                  size="sm"
                  variant={viewMode === 'month' ? 'default' : 'ghost'}
                  onClick={() => setViewMode('month')}
                >
                  Month
                </Button>
                <Button
                  size="sm"
                  variant={viewMode === 'week' ? 'default' : 'ghost'}
                  onClick={() => setViewMode('week')}
                >
                  Week
                </Button>
                <Button
                  size="sm"
                  variant={viewMode === 'day' ? 'default' : 'ghost'}
                  onClick={() => setViewMode('day')}
                >
                  Day
                </Button>
              </div>
              
              <Button
                size="sm"
                variant="outline"
                onClick={() => setSelectedDate(new Date())}
              >
                Today
              </Button>
              
              {onOpenInNewTab && (
                <Button size="sm" variant="ghost" onClick={onOpenInNewTab}>
                  <ExternalLink className="h-4 w-4" />
                </Button>
              )}
            </div>
          </div>

          {/* Date Navigation */}
          <div className="flex items-center justify-between mt-4">
            <div className="flex items-center gap-2">
              <Button size="sm" variant="ghost" onClick={() => navigateDate('prev')}>
                <ChevronLeft className="h-4 w-4" />
              </Button>
              <h2 className="text-xl font-semibold text-slate-900">
                {format(selectedDate, viewMode === 'month' ? 'MMMM yyyy' : viewMode === 'week' ? 'MMM dd, yyyy' : 'EEEE, MMM dd, yyyy')}
              </h2>
              <Button size="sm" variant="ghost" onClick={() => navigateDate('next')}>
                <ChevronRight className="h-4 w-4" />
              </Button>
            </div>
            
            <div className="text-sm text-slate-600">
              {currentMachine.nameMyanmar}
            </div>
          </div>
        </div>

        {/* Calendar Content */}
        <div className="flex-1 overflow-auto">
          <div className="p-6">
            {viewMode === 'month' && renderMonthView()}
            {viewMode === 'week' && renderWeekView()}
            {viewMode === 'day' && renderDayViewTimeline()}
          </div>
        </div>
      </div>

      {/* Sidebar - Show in day view for both modes */}
      {viewMode === 'day' && (
        <div className="w-80 bg-white border-l border-slate-200">
          <DroppableSidebar onDrop={handleSidebarDrop} mode={mode}>
            <div className="p-4 border-b border-slate-200">
              <h3 className="font-semibold text-slate-900 mb-1">
                {mode === 'planning' ? 'Unscheduled Plans' : 'Job Queue'} — {currentMachine.name}
              </h3>
              <p className="text-sm text-slate-600 mb-2">
                {mode === 'planning' ? 'မစီစဉ်ရသေးသော အစီအစဉ်များ' : 'အလုပ်စာရင်း'}
              </p>
              <div className={`text-xs px-2 py-2 rounded border ${
                mode === 'planning' 
                  ? 'text-blue-600 bg-blue-50 border-blue-200' 
                  : 'text-orange-600 bg-orange-50 border-orange-200'
              }`}>
                💡 <strong>Tip:</strong> {mode === 'planning' 
                  ? 'Drag draft/final plans to schedule them on timeline • Right-click to transfer to other machines' 
                  : 'Drag approved jobs from timeline back here to unschedule them • Right-click to transfer machines'
                }
              </div>
            </div>
            
            <div className="max-h-[calc(100vh-200px)] overflow-y-auto">
              
              {/* 1. Draft Plans Section */}
              <div className="p-4 border-b border-slate-100">
                <div className="flex items-center justify-between mb-3">
                  <h4 className="font-medium text-slate-900 flex items-center gap-2">
                    <div className="w-3 h-3 bg-blue-500 rounded"></div>
                    Draft Plans ({getUnscheduledPlans().filter(p => p.status === 'draft').length})
                  </h4>
                </div>
                {mode === 'planning' && getUnscheduledPlans().filter(p => p.status === 'draft').length > 0 && (
                  <div className="text-xs mb-3 px-2 py-1 rounded text-blue-600 bg-blue-50 border border-blue-200">
                    📱 Drag to timeline to schedule
                  </div>
                )}
                <div className="space-y-2">
                  {getUnscheduledPlans().filter(p => p.status === 'draft').map((plan) => {
                    const compatibleMachines = getCompatibleMachines(machineId);
                    console.log('🔵 RENDERING DRAFT PLAN in sidebar:', {
                      id: plan.id,
                      productName: plan.productName,
                      startTime: plan.startTime || 'NONE',
                      endTime: plan.endTime || 'NONE',
                      isScheduled: !!(plan.startTime && plan.endTime)
                    });
                    
                    return (
                      <div key={plan.id}>
                        {mode === 'planning' ? (
                          <ContextMenu>
                            <ContextMenuTrigger>
                              <DraggablePlan plan={plan} mode={mode}>
                                <div className="p-2 border border-blue-200 rounded-lg bg-blue-50 hover:shadow-md transition-all cursor-move">
                                  <div className="flex items-start gap-2">
                                    <div className="w-8 h-8 rounded overflow-hidden bg-slate-200 flex-shrink-0">
                                      <ImageWithFallback
                                        src={plan.productImage}
                                        alt={plan.productName}
                                        className="w-full h-full object-cover"
                                      />
                                    </div>
                                    <div className="flex-1 min-w-0">
                                      <div className="flex items-center justify-between mb-1">
                                        <div className="flex items-center gap-1">
                                          <div className="w-2 h-2 bg-blue-500 rounded-full"></div>
                                          <span className="text-xs font-medium text-blue-700">Plan ID: {plan.id}</span>
                                        </div>
                                        <div className="flex items-center gap-1">
                                          <GripVertical className="h-3 w-3 text-blue-400" />
                                          <ArrowRightLeft className="h-3 w-3 text-blue-500" title="Right-click to transfer machine" />
                                        </div>
                                      </div>
                                      <h5 className="text-xs font-medium text-slate-700 truncate">{plan.productName}</h5>
                                      <p className="text-xs text-slate-600">{plan.quantity.toLocaleString()} pieces</p>
                                      <div className="flex items-center justify-between mt-1">
                                        <Badge className="bg-blue-500 text-white text-xs">
                                          {getStatusIcon(plan.status)} Draft
                                        </Badge>
                                        <span className="text-xs text-slate-500">{plan.plannerName}</span>
                                      </div>
                                      <div className="mt-1 text-xs text-blue-600 flex items-center justify-between">
                                        <div className="flex items-center gap-1">
                                          <span>📱</span>
                                          <span>Drag to timeline to schedule</span>
                                        </div>
                                        <span className="text-blue-500">Right-click to transfer</span>
                                      </div>
                                    </div>
                                  </div>
                                </div>
                              </DraggablePlan>
                            </ContextMenuTrigger>
                            <ContextMenuContent className="w-56">
                              <ContextMenuItem className="flex items-center gap-2 text-slate-600 font-medium cursor-default">
                                <ArrowRightLeft className="h-4 w-4" />
                                Transfer to Compatible Machine
                              </ContextMenuItem>
                              <ContextMenuSeparator />
                              {compatibleMachines.length > 0 ? (
                                compatibleMachines.map((targetMachineId) => {
                                  const targetMachine = mockMachines[targetMachineId];
                                  if (!targetMachine) return null;
                                  
                                  return (
                                    <ContextMenuItem
                                      key={targetMachineId}
                                      className="flex items-center gap-3 py-2"
                                      onClick={() => handlePlanTransfer(plan, targetMachineId)}
                                    >
                                      <div className="flex items-center gap-2 flex-1">
                                        <div className={`w-2 h-2 rounded-full ${
                                          targetMachine.status === 'running' ? 'bg-green-500' :
                                          targetMachine.status === 'idle' ? 'bg-yellow-500' :
                                          targetMachine.status === 'down' ? 'bg-red-500' :
                                          'bg-gray-500'
                                        }`}></div>
                                        <span className="font-medium text-slate-900">{targetMachine.name}</span>
                                      </div>
                                      <div className="text-right">
                                        <div className="text-xs text-slate-500">{targetMachine.nameMyanmar}</div>
                                        <div className="text-xs font-medium text-slate-600">
                                          {targetMachine.capacity.toLocaleString()} cap
                                        </div>
                                      </div>
                                    </ContextMenuItem>
                                  );
                                })
                              ) : (
                                <ContextMenuItem className="text-slate-500 cursor-default">
                                  No compatible machines available
                                </ContextMenuItem>
                              )}
                            </ContextMenuContent>
                          </ContextMenu>
                        ) : (
                          <div className="p-2 border border-slate-200 rounded-lg bg-slate-50 opacity-60">
                            <div className="flex items-start gap-2">
                              <div className="w-8 h-8 rounded overflow-hidden bg-slate-200 flex-shrink-0">
                                <ImageWithFallback
                                  src={plan.productImage}
                                  alt={plan.productName}
                                  className="w-full h-full object-cover"
                                />
                              </div>
                              <div className="flex-1 min-w-0">
                                <h5 className="text-xs font-medium text-slate-700 truncate">{plan.productName}</h5>
                                <p className="text-xs text-slate-500">{plan.quantity.toLocaleString()} pcs</p>
                              </div>
                            </div>
                          </div>
                        )}
                      </div>
                    );
                  })}
                </div>
              </div>

              {/* 2. Final Plans Section */}
              <div className="p-4 border-b border-slate-100">
                <div className="flex items-center justify-between mb-3">
                  <h4 className="font-medium text-slate-900 flex items-center gap-2">
                    <div className="w-3 h-3 bg-yellow-400 rounded"></div>
                    Final Plans ({getUnscheduledPlans().filter(p => p.status === 'final').length})
                  </h4>
                </div>
                {mode === 'planning' && getUnscheduledPlans().filter(p => p.status === 'final').length > 0 && (
                  <div className="text-xs mb-3 px-2 py-1 rounded text-yellow-600 bg-yellow-50 border border-yellow-200">
                    📱 Drag to timeline to schedule
                  </div>
                )}
                <div className="space-y-2">
                  {getUnscheduledPlans().filter(p => p.status === 'final').map((plan) => {
                    const compatibleMachines = getCompatibleMachines(machineId);
                    
                    return (
                      <div key={plan.id}>
                        {mode === 'planning' ? (
                          <ContextMenu>
                            <ContextMenuTrigger>
                              <DraggablePlan plan={plan} mode={mode}>
                                <div className="p-2 border border-yellow-200 rounded-lg bg-yellow-50 hover:shadow-md transition-all cursor-move">
                                  <div className="flex items-start gap-2">
                                    <div className="w-8 h-8 rounded overflow-hidden bg-slate-200 flex-shrink-0">
                                      <ImageWithFallback
                                        src={plan.productImage}
                                        alt={plan.productName}
                                        className="w-full h-full object-cover"
                                      />
                                    </div>
                                    <div className="flex-1 min-w-0">
                                      <div className="flex items-center justify-between mb-1">
                                        <div className="flex items-center gap-1">
                                          <div className="w-2 h-2 bg-yellow-500 rounded-full"></div>
                                          <span className="text-xs font-medium text-yellow-700">Plan ID: {plan.id}</span>
                                        </div>
                                        <div className="flex items-center gap-1">
                                          <GripVertical className="h-3 w-3 text-yellow-400" />
                                          <ArrowRightLeft className="h-3 w-3 text-yellow-500" title="Right-click to transfer machine" />
                                        </div>
                                      </div>
                                      <h5 className="text-xs font-medium text-slate-700 truncate">{plan.productName}</h5>
                                      <p className="text-xs text-slate-600">{plan.quantity.toLocaleString()} pieces</p>
                                      <div className="flex items-center justify-between mt-1">
                                        <Badge className="bg-yellow-500 text-white text-xs">
                                          {getStatusIcon(plan.status)} Final
                                        </Badge>
                                        <span className="text-xs text-slate-500">{plan.plannerName}</span>
                                      </div>
                                      <div className="mt-1 text-xs text-yellow-600 flex items-center justify-between">
                                        <div className="flex items-center gap-1">
                                          <span>📱</span>
                                          <span>Drag to timeline to schedule</span>
                                        </div>
                                        <span className="text-yellow-500">Right-click to transfer</span>
                                      </div>
                                    </div>
                                  </div>
                                </div>
                              </DraggablePlan>
                            </ContextMenuTrigger>
                            <ContextMenuContent className="w-56">
                              <ContextMenuItem className="flex items-center gap-2 text-slate-600 font-medium cursor-default">
                                <ArrowRightLeft className="h-4 w-4" />
                                Transfer to Compatible Machine
                              </ContextMenuItem>
                              <ContextMenuSeparator />
                              {compatibleMachines.length > 0 ? (
                                compatibleMachines.map((targetMachineId) => {
                                  const targetMachine = mockMachines[targetMachineId];
                                  if (!targetMachine) return null;
                                  
                                  return (
                                    <ContextMenuItem
                                      key={targetMachineId}
                                      className="flex items-center gap-3 py-2"
                                      onClick={() => handlePlanTransfer(plan, targetMachineId)}
                                    >
                                      <div className="flex items-center gap-2 flex-1">
                                        <div className={`w-2 h-2 rounded-full ${
                                          targetMachine.status === 'running' ? 'bg-green-500' :
                                          targetMachine.status === 'idle' ? 'bg-yellow-500' :
                                          targetMachine.status === 'down' ? 'bg-red-500' :
                                          'bg-gray-500'
                                        }`}></div>
                                        <span className="font-medium text-slate-900">{targetMachine.name}</span>
                                      </div>
                                      <div className="text-right">
                                        <div className="text-xs text-slate-500">{targetMachine.nameMyanmar}</div>
                                        <div className="text-xs font-medium text-slate-600">
                                          {targetMachine.capacity.toLocaleString()} cap
                                        </div>
                                      </div>
                                    </ContextMenuItem>
                                  );
                                })
                              ) : (
                                <ContextMenuItem className="text-slate-500 cursor-default">
                                  No compatible machines available
                                </ContextMenuItem>
                              )}
                            </ContextMenuContent>
                          </ContextMenu>
                        ) : (
                          <div className="p-2 border border-slate-200 rounded-lg bg-slate-50 opacity-60">
                            <div className="flex items-start gap-2">
                              <div className="w-8 h-8 rounded overflow-hidden bg-slate-200 flex-shrink-0">
                                <ImageWithFallback
                                  src={plan.productImage}
                                  alt={plan.productName}
                                  className="w-full h-full object-cover"
                                />
                              </div>
                              <div className="flex-1 min-w-0">
                                <h5 className="text-xs font-medium text-slate-700 truncate">{plan.productName}</h5>
                                <p className="text-xs text-slate-500">{plan.quantity.toLocaleString()} pcs</p>
                              </div>
                            </div>
                          </div>
                        )}
                      </div>
                    );
                  })}
                </div>
              </div>

              {/* 3. Approved Jobs Section */}
              <div className="p-4 border-b border-slate-100">
                <div className="flex items-center justify-between mb-3">
                  <h4 className="font-medium text-slate-900 flex items-center gap-2">
                    <div className="w-3 h-3 bg-orange-500 rounded"></div>
                    Approved Jobs ({getAllUnscheduledApprovedJobs().length})
                  </h4>
                </div>
                {mode === 'planning' ? (
                  <div className="text-xs mb-3 px-2 py-1 rounded text-slate-500 bg-slate-50 border border-slate-200">
                    👁️ View Only • Cannot drag (Production Control handles these)
                  </div>
                ) : (
                  getAllUnscheduledApprovedJobs().length > 0 && (
                    <div className="grid grid-cols-1 gap-1 mb-3">
                      <div className="text-xs text-green-600 px-2 py-1 bg-green-50 rounded flex items-center gap-1">
                        <GripVertical className="h-3 w-3" />
                        Drag to Timeline
                      </div>
                      <div className="text-xs text-blue-600 px-2 py-1 bg-blue-50 rounded flex items-center gap-1">
                        <ArrowRightLeft className="h-3 w-3" />
                        Right-click to Transfer Machine
                      </div>
                    </div>
                  )
                )}
                <div className="space-y-2">
                  {getAllUnscheduledApprovedJobs().map((plan) => {
                    const compatibleMachines = getCompatibleMachines(machineId);
                    
                    if (mode === 'planning') {
                      // View-only in planning mode
                      return (
                        <div key={plan.id} className="p-2 border border-slate-200 rounded-lg bg-slate-50 opacity-60">
                          <div className="flex items-start gap-2">
                            <div className="w-8 h-8 rounded overflow-hidden bg-slate-200 flex-shrink-0">
                              <ImageWithFallback
                                src={plan.productImage}
                                alt={plan.productName}
                                className="w-full h-full object-cover"
                              />
                            </div>
                            <div className="flex-1 min-w-0">
                              <div className="flex items-center gap-1 mb-1">
                                <div className="w-2 h-2 bg-orange-500 rounded-full"></div>
                                <span className="text-xs font-bold text-slate-600">{plan.jobId}</span>
                              </div>
                              <h5 className="text-xs font-medium text-slate-700 truncate">{plan.productName}</h5>
                              <p className="text-xs text-slate-500">{plan.quantity.toLocaleString()} pieces</p>
                              <div className="text-xs text-slate-400 mt-1">Plan ID: {plan.id}</div>
                            </div>
                          </div>
                        </div>
                      );
                    }
                    
                    // Draggable in production control mode
                    return (
                      <ContextMenu key={plan.id}>
                        <ContextMenuTrigger>
                          <DraggablePlan plan={plan} mode={mode}>
                            <div className="p-2 border border-orange-200 rounded-lg bg-orange-50 hover:shadow-md transition-all cursor-move">
                              <div className="flex items-start gap-2">
                                <div className="w-8 h-8 rounded overflow-hidden bg-slate-200 flex-shrink-0">
                                  <ImageWithFallback
                                    src={plan.productImage}
                                    alt={plan.productName}
                                    className="w-full h-full object-cover"
                                  />
                                </div>
                                <div className="flex-1 min-w-0">
                                  <div className="flex items-center justify-between mb-1">
                                    <div className="flex items-center gap-1">
                                      <div className="w-2 h-2 bg-orange-500 rounded-full"></div>
                                      <span className="text-xs font-bold text-orange-700">{plan.jobId}</span>
                                    </div>
                                    <div className="flex items-center gap-1">
                                      <GripVertical className="h-3 w-3 text-orange-400" />
                                      <ArrowRightLeft className="h-3 w-3 text-blue-500" title="Right-click to transfer" />
                                    </div>
                                  </div>
                                  <h5 className="text-xs font-medium text-slate-700 truncate">{plan.productName}</h5>
                                  <p className="text-xs text-slate-500">{plan.quantity.toLocaleString()} pcs</p>
                                  <div className="text-xs text-slate-400 mt-1">Plan ID: {plan.id}</div>
                                </div>
                              </div>
                            </div>
                          </DraggablePlan>
                        </ContextMenuTrigger>
                        <ContextMenuContent className="w-56">
                          <ContextMenuItem className="flex items-center gap-2 text-slate-600 font-medium cursor-default">
                            <ArrowRightLeft className="h-4 w-4" />
                            Transfer to Compatible Machine
                          </ContextMenuItem>
                          <ContextMenuSeparator />
                          {compatibleMachines.length > 0 ? (
                            compatibleMachines.map((targetMachineId) => {
                              const targetMachine = mockMachines[targetMachineId];
                              if (!targetMachine) return null;
                              
                              return (
                                <ContextMenuItem
                                  key={targetMachineId}
                                  className="flex items-center gap-3 py-2"
                                  onClick={() => handleQuickTransfer(plan, targetMachineId)}
                                >
                                  <div className="flex items-center gap-2 flex-1">
                                    <div className={`w-2 h-2 rounded-full ${
                                      targetMachine.status === 'running' ? 'bg-green-500' :
                                      targetMachine.status === 'idle' ? 'bg-yellow-500' :
                                      targetMachine.status === 'down' ? 'bg-red-500' :
                                      'bg-gray-500'
                                    }`}></div>
                                    <span className="font-medium text-slate-900">{targetMachine.name}</span>
                                  </div>
                                  <div className="text-right">
                                    <div className="text-xs text-slate-500">{targetMachine.nameMyanmar}</div>
                                    <div className="text-xs font-medium text-slate-600">
                                      {targetMachine.capacity.toLocaleString()} cap
                                    </div>
                                  </div>
                                </ContextMenuItem>
                              );
                            })
                          ) : (
                            <ContextMenuItem className="text-slate-500 cursor-default">
                              No compatible machines available
                            </ContextMenuItem>
                          )}
                        </ContextMenuContent>
                      </ContextMenu>
                    );
                  })}
                </div>
              </div>

              {/* 4. Running Section */}
              <div className="p-4 border-b border-slate-100">
                <div className="flex items-center justify-between mb-3">
                  <h4 className="font-medium text-slate-900 flex items-center gap-2">
                    <div className="w-3 h-3 bg-green-500 rounded"></div>
                    Running ({getPlansForDate(selectedDate).filter(p => p.status === 'running').length})
                  </h4>
                </div>
                <div className="text-xs text-slate-500 mb-3 px-2 py-1 bg-slate-50 rounded">
                  Read-only • Must Pause first to move
                </div>
                <div className="space-y-2">
                  {getPlansForDate(selectedDate).filter(p => p.status === 'running').map((plan) => (
                    <div key={plan.id} className="p-2 border border-green-200 rounded-lg bg-green-50">
                      <div className="flex items-start gap-2">
                        <div className="w-8 h-8 rounded overflow-hidden bg-slate-200 flex-shrink-0">
                          <ImageWithFallback
                            src={plan.productImage}
                            alt={plan.productName}
                            className="w-full h-full object-cover"
                          />
                        </div>
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-1 mb-1">
                            <div className="w-2 h-2 bg-green-500 rounded-full animate-pulse"></div>
                            <span className="text-xs font-bold text-green-700">{plan.jobId}</span>
                          </div>
                          <h5 className="text-xs font-medium text-slate-700 truncate">{plan.productName}</h5>
                          <p className="text-xs text-slate-500">{plan.quantity.toLocaleString()} pieces</p>
                          <div className="text-xs text-slate-400 mt-1">Plan ID: {plan.id}</div>
                          <div className="flex items-center gap-1 mt-1">
                            <Activity className="h-3 w-3 text-green-500" />
                            <span className="text-xs text-green-600">In Progress</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              {/* 5. Completed Section */}
              <div className="p-4">
                <div className="flex items-center justify-between mb-3">
                  <h4 className="font-medium text-slate-900 flex items-center gap-2">
                    <div className="w-3 h-3 bg-green-800 rounded"></div>
                    Completed ({getPlansForDate(selectedDate).filter(p => p.status === 'completed').length})
                  </h4>
                </div>
                <div className="text-xs text-slate-500 mb-3 px-2 py-1 bg-slate-50 rounded">
                  Read-only History
                </div>
                <div className="space-y-2">
                  {getPlansForDate(selectedDate).filter(p => p.status === 'completed').map((plan) => (
                    <div key={plan.id} className="p-2 border border-green-300 rounded-lg bg-green-100">
                      <div className="flex items-start gap-2">
                        <div className="w-8 h-8 rounded overflow-hidden bg-slate-200 flex-shrink-0">
                          <ImageWithFallback
                            src={plan.productImage}
                            alt={plan.productName}
                            className="w-full h-full object-cover"
                          />
                        </div>
                        <div className="flex-1 min-w-0">
                          <div className="flex items-center gap-1 mb-1">
                            <div className="w-2 h-2 bg-green-800 rounded-full"></div>
                            <span className="text-xs font-bold text-green-800">{plan.jobId}</span>
                          </div>
                          <h5 className="text-xs font-medium text-slate-700 truncate">{plan.productName}</h5>
                          <p className="text-xs text-slate-500">{plan.quantity.toLocaleString()} pieces</p>
                          <div className="text-xs text-slate-400 mt-1">Plan ID: {plan.id}</div>
                          <div className="flex items-center gap-1 mt-1">
                            <CheckCircle2 className="h-3 w-3 text-green-600" />
                            <span className="text-xs text-green-600">Completed</span>
                          </div>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </div>

              {/* Empty State Message - Only show if no items in any section */}
              {getUnscheduledPlans().length === 0 && 
               getAllUnscheduledApprovedJobs().length === 0 && 
               getPlansForDate(selectedDate).filter(p => p.status === 'running').length === 0 && 
               getPlansForDate(selectedDate).filter(p => p.status === 'completed').length === 0 && (
                <div className="text-center py-8">
                  <div className="text-slate-400 mb-2">
                    <Package className="h-8 w-8 mx-auto" />
                  </div>
                  <p className="text-sm text-slate-500">
                    No plans available
                  </p>
                  <p className="text-xs text-slate-400 mt-1">
                    အစီအစဉ်များ မရှိပါ
                  </p>
                </div>
              )}


            </div>
          </DroppableSidebar>
        </div>
      )}

      {/* Remove duplicate planning sidebar - now using unified sidebar */}
      {false && (
        <div className="w-80 bg-white border-l border-slate-200">
          <DroppableSidebar onDrop={handleSidebarDrop} mode={mode}>
            <div className="p-4 border-b border-slate-200">
              <h3 className="font-semibold text-slate-900 mb-1">
                Unscheduled Plans — {currentMachine.name}
              </h3>
              <p className="text-sm text-slate-600 mb-2">
                မစီစဉ်ရသေးသော အစီအစဉ်များ
              </p>
              <div className="text-xs text-blue-600 bg-blue-50 p-2 rounded border border-blue-200">
                💡 <strong>Tip:</strong> Drag scheduled plans from timeline back here to unschedule them
              </div>
            </div>
            
            <div className="p-4 space-y-3 max-h-[calc(100vh-200px)] overflow-y-auto">
              {getUnscheduledPlans().map((plan) => (
                <DraggablePlan key={plan.id} plan={plan} mode={mode}>
                  <Card className="cursor-move hover:shadow-md transition-all duration-200 border border-slate-200 bg-white hover:border-blue-300">
                    <CardContent className="p-3">
                      <div className="flex items-start gap-3">
                        <div className="w-12 h-12 rounded-lg overflow-hidden bg-slate-100 border flex-shrink-0">
                          <ImageWithFallback
                            src={plan.productImage}
                            alt={plan.productName}
                            className="w-full h-full object-cover"
                          />
                        </div>
                        <div className="flex-1 min-w-0">
                          <div className="flex items-start justify-between gap-2">
                            <h4 className="font-medium text-slate-900 leading-tight">
                              {plan.productName}
                            </h4>
                            <GripVertical className="h-4 w-4 text-slate-400 flex-shrink-0 mt-0.5" />
                          </div>
                          <p className="text-sm text-slate-600 mt-1">
                            {plan.quantity.toLocaleString()} pieces
                          </p>
                          <p className="text-xs text-slate-500 mt-1">
                            Plan ID: {plan.id}
                          </p>
                          <div className="flex items-center justify-between mt-2">
                            <Badge 
                              variant="secondary" 
                              className={`${getStatusColor(plan.status)} ${getStatusTextColor(plan.status)} border-0 text-xs`}
                            >
                              {getStatusIcon(plan.status)} {plan.status.charAt(0).toUpperCase() + plan.status.slice(1)}
                            </Badge>
                            <span className="text-xs text-slate-500">
                              {plan.plannerName}
                            </span>
                          </div>
                          <div className="mt-2 text-xs text-blue-600 flex items-center gap-1">
                            <span>📱</span>
                            <span>Drag to timeline to schedule</span>
                          </div>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                </DraggablePlan>
              ))}
              
              {getUnscheduledPlans().length === 0 && (
                <div className="text-center py-8">
                  <div className="text-slate-400 mb-2">
                    <Package className="h-8 w-8 mx-auto" />
                  </div>
                  <p className="text-sm text-slate-500">
                    No unscheduled plans
                  </p>
                  <p className="text-xs text-slate-400 mt-1">
                    မစီစဉ်ရသေး��ော အ���ီအစဉ်များ မရှိပါ
                  </p>
                </div>
              )}
            </div>
          </DroppableSidebar>
        </div>
      )}

      {/* Reschedule Confirmation Modal - Production Control Only */}
      {mode === 'production-control' && (
        <Dialog open={rescheduleModalOpen} onOpenChange={setRescheduleModalOpen}>
          <DialogContent className="sm:max-w-[425px]">
            <DialogHeader>
              <DialogTitle>Reschedule Job</DialogTitle>
              <DialogDescription>
                Confirm the rescheduling of this job and provide a reason.
              </DialogDescription>
            </DialogHeader>
            
            {rescheduleData && (
              <div className="space-y-4">
                <div className="bg-slate-50 p-3 rounded-lg">
                  <h4 className="font-medium text-slate-900 mb-2">Job Details</h4>
                  <div className="space-y-1 text-sm">
                    <div className="flex justify-between">
                      <span className="text-slate-600">Job ID:</span>
                      <span className="font-medium">{rescheduleData.plan.jobId}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-slate-600">Product:</span>
                      <span className="font-medium">{rescheduleData.plan.productName}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-slate-600">From:</span>
                      <span className="font-medium">{rescheduleData.fromTime}</span>
                    </div>
                    <div className="flex justify-between">
                      <span className="text-slate-600">To:</span>
                      <span className="font-medium text-blue-600">{rescheduleData.toTime}</span>
                    </div>
                  </div>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="reason">Reason for Reschedule *</Label>
                  <Select value={rescheduleReason} onValueChange={setRescheduleReason}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select a reason" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="material-delay">Material Delay</SelectItem>
                      <SelectItem value="mold-change">Mold Change Required</SelectItem>
                      <SelectItem value="operator-unavailable">Operator Unavailable</SelectItem>
                      <SelectItem value="machine-down">Machine Down</SelectItem>
                      <SelectItem value="priority-change">Priority Change</SelectItem>
                      <SelectItem value="other">Other</SelectItem>
                    </SelectContent>
                  </Select>
                </div>

                <div className="space-y-2">
                  <Label htmlFor="note">Additional Notes (Optional)</Label>
                  <Textarea
                    id="note"
                    placeholder="Enter any additional details..."
                    value={rescheduleNote}
                    onChange={(e) => setRescheduleNote(e.target.value)}
                    rows={3}
                  />
                </div>
              </div>
            )}

            <DialogFooter>
              <Button 
                variant="outline" 
                onClick={() => {
                  setRescheduleModalOpen(false);
                  setRescheduleData(null);
                  setRescheduleReason('');
                  setRescheduleNote('');
                }}
              >
                Cancel
              </Button>
              <Button 
                onClick={handleRescheduleConfirm}
                disabled={!rescheduleReason}
              >
                Confirm Reschedule
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      )}

      {/* Machine Transfer Confirmation Dialog */}
      {transferModalOpen && transferData && (
        <Dialog open={transferModalOpen} onOpenChange={setTransferModalOpen}>
          <DialogContent className="sm:max-w-md">
            <DialogHeader>
              <DialogTitle className="flex items-center gap-2">
                <div className="w-8 h-8 bg-orange-100 rounded-full flex items-center justify-center">
                  <GripVertical className="h-4 w-4 text-orange-600" />
                </div>
                Machine Transfer Confirmation
              </DialogTitle>
              <DialogDescription>
                Transfer this approved job to a different machine and provide a reason.
              </DialogDescription>
            </DialogHeader>
            
            <div className="space-y-4">
              <div className="bg-orange-50 p-3 rounded-lg border border-orange-200">
                <h4 className="font-medium text-slate-900 mb-2">Transfer Details</h4>
                <div className="space-y-1 text-sm">
                  <div className="flex justify-between">
                    <span className="text-slate-600">Job ID:</span>
                    <span className="font-medium text-orange-600">{transferData.jobId}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-slate-600">Product:</span>
                    <span className="font-medium">{transferData.productName}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-slate-600">Quantity:</span>
                    <span className="font-medium">{transferData.quantity.toLocaleString()} pcs</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-slate-600">From Machine:</span>
                    <span className="font-medium">{mockMachines[transferData.fromMachineId]?.name}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-slate-600">To Machine:</span>
                    <span className="font-medium text-blue-600">{mockMachines[transferData.toMachineId]?.name}</span>
                  </div>
                </div>
              </div>

              <div className="space-y-2">
                <Label htmlFor="transfer-reason">Reason for Transfer *</Label>
                <Select value={transferReason} onValueChange={setTransferReason}>
                  <SelectTrigger>
                    <SelectValue placeholder="Select a reason" />
                  </SelectTrigger>
                  <SelectContent>
                    <SelectItem value="machine-optimization">Machine Optimization</SelectItem>
                    <SelectItem value="capacity-balancing">Capacity Balancing</SelectItem>
                    <SelectItem value="machine-maintenance">Source Machine Maintenance</SelectItem>
                    <SelectItem value="operator-availability">Operator Availability</SelectItem>
                    <SelectItem value="quality-requirements">Quality Requirements</SelectItem>
                    <SelectItem value="urgent-priority">Urgent Priority Job</SelectItem>
                    <SelectItem value="other">Other</SelectItem>
                  </SelectContent>
                </Select>
              </div>

              {getCompatibleMachines(transferData.fromMachineId).includes(transferData.toMachineId) ? (
                <div className="flex items-center gap-2 text-green-600 text-sm bg-green-50 p-2 rounded">
                  <CheckCircle2 className="h-4 w-4" />
                  <span>Machines are compatible for this transfer</span>
                </div>
              ) : (
                <div className="flex items-center gap-2 text-amber-600 text-sm bg-amber-50 p-2 rounded">
                  <AlertTriangle className="h-4 w-4" />
                  <span>⚠️ Check machine compatibility before confirming</span>
                </div>
              )}
            </div>

            <DialogFooter>
              <Button 
                variant="outline" 
                onClick={() => {
                  setTransferModalOpen(false);
                  setTransferData(null);
                  setTransferReason('');
                }}
              >
                Cancel
              </Button>
              <Button 
                onClick={handleTransferConfirm}
                disabled={!transferReason}
                className="bg-orange-600 hover:bg-orange-700"
              >
                Confirm Transfer
              </Button>
            </DialogFooter>
          </DialogContent>
        </Dialog>
      )}
    </div>
  );
}

export default MachineCalendarView;