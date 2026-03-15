import React, { createContext, useContext, useState, useCallback } from 'react';

export interface ProductionPlan {
  id: string;
  productId: string;
  productName: string;
  productImage: string;
  quantity: number;
  startTime: string;
  endTime: string;
  date: Date;
  status: 'draft' | 'final' | 'approved' | 'running' | 'paused' | 'completed';
  operatorId?: string;
  operatorName?: string;
  plannerName?: string;
  jobId?: string;
  moldId?: string;
  machineId: string; // Added to track which machine the job belongs to
  materialRequirements?: {
    glue: number;
    color: string;
    bags: number;
  };
  notes?: string;
}

export interface MoldChangeRequest {
  id: string;
  fromMoldId: string;
  toMoldId: string;
  fromMoldName: string;
  toMoldName: string;
  machineId: string;
  machineName: string;
  jobId?: string;
  productId: string;
  productName: string;
  status: 'planned' | 'pending' | 'approved' | 'rejected' | 'in-progress' | 'completed';
  estimatedTime: number; // in minutes
  actualTime?: number; // in minutes
  createdBy: string;
  createdAt: Date;
  scheduledDate: Date;
  scheduledTime: string;
  approvedBy?: string;
  approvedAt?: Date;
  rejectedBy?: string;
  rejectedAt?: Date;
  rejectionReason?: string;
  acceptedBy?: string;
  acceptedAt?: Date;
  startedBy?: string;
  startedAt?: Date;
  completedBy?: string;
  completedAt?: Date;
  notes?: string;
}

// Product to Mold mapping
export const productMoldMapping: Record<string, string> = {
  'PRD-001': 'MOLD-001', // Plastic Cup 16oz Clear
  'PRD-002': 'MOLD-002', // Food Container 1L
  'PRD-003': 'MOLD-003', // Disposable Bowl 32oz
  'PRD-004': 'MOLD-001', // Plastic Cup 12oz Blue (same mold as PRD-001)
  'PRD-005': 'MOLD-004', // Water Bottle 500ml
  'PRD-006': 'MOLD-005', // Disposable Plate 9inch
  'PRD-007': 'MOLD-006', // Lunch Box Kids
  'PRD-008': 'MOLD-007', // Plastic Spoon Set
  'PRD-009': 'MOLD-004', // Bottle 1L Green (same mold as PRD-005)
};

// Mold information
export const moldInfo: Record<string, { name: string; changeTime: number }> = {
  'MOLD-001': { name: 'Cup Mold Standard', changeTime: 45 },
  'MOLD-002': { name: 'Container Mold 1L', changeTime: 60 },
  'MOLD-003': { name: 'Bowl Mold Large', changeTime: 50 },
  'MOLD-004': { name: 'Bottle Mold Universal', changeTime: 55 },
  'MOLD-005': { name: 'Plate Mold 9inch', changeTime: 40 },
  'MOLD-006': { name: 'Lunch Box Mold', changeTime: 70 },
  'MOLD-007': { name: 'Cutlery Mold Set', changeTime: 35 },
};

// Machine current mold assignments (in real system this would come from DB)
export const machineMoldAssignments: Record<string, string> = {
  'IM-001': 'MOLD-001',
  'IM-002': 'MOLD-002', 
  'IM-003': 'MOLD-003',
  'IM-004': 'MOLD-004',
  'IM-005': 'MOLD-005',
  'IM-006': 'MOLD-006',
};

interface JobScheduleContextType {
  plans: ProductionPlan[];
  setPlans: React.Dispatch<React.SetStateAction<ProductionPlan[]>>;
  mcrRequests: MoldChangeRequest[];
  setMcrRequests: React.Dispatch<React.SetStateAction<MoldChangeRequest[]>>;
  transferJobToMachine: (jobId: string, fromMachineId: string, toMachineId: string, newDate?: Date, mode?: 'planning' | 'production-control') => boolean;
  getPlansForMachine: (machineId: string) => ProductionPlan[];
  updatePlan: (planId: string, updates: Partial<ProductionPlan>) => void;
  checkMoldChange: (productId: string, targetMachineId: string) => boolean;
  createMCR: (params: {
    fromMoldId: string;
    toMoldId: string;
    machineId: string;
    jobId?: string;
    productId: string;
    productName: string;
    scheduledDate: Date;
    scheduledTime: string;
    mode: 'planning' | 'production-control';
    createdBy: string;
  }) => string;
  getMCRsForMachine: (machineId: string) => MoldChangeRequest[];
  updateMCR: (mcrId: string, updates: Partial<MoldChangeRequest>) => void;
  convertPlannedMCRsToPending: () => void;
}

const JobScheduleContext = createContext<JobScheduleContextType | undefined>(undefined);

export function JobScheduleProvider({ children }: { children: React.ReactNode }) {
  const [plans, setPlans] = useState<ProductionPlan[]>([]);
  const [mcrRequests, setMcrRequests] = useState<MoldChangeRequest[]>([]);

  const transferJobToMachine = useCallback((
    jobId: string, 
    fromMachineId: string, 
    toMachineId: string, 
    newDate?: Date,
    mode: 'planning' | 'production-control' = 'planning'
  ): boolean => {
    let transferSuccess = false;
    
    setPlans(prevPlans => {
      const planToTransfer = prevPlans.find(plan => plan.jobId === jobId && plan.machineId === fromMachineId);
      if (!planToTransfer) return prevPlans;

      // Check if mold change is needed
      const needsMoldChange = checkMoldChange(planToTransfer.productId, toMachineId);
      
      if (needsMoldChange) {
        const fromMoldId = machineMoldAssignments[fromMachineId];
        const toMoldId = productMoldMapping[planToTransfer.productId];
        
        // Create MCR
        createMCR({
          fromMoldId: machineMoldAssignments[toMachineId], // Current mold on target machine
          toMoldId,
          machineId: toMachineId,
          jobId: planToTransfer.jobId,
          productId: planToTransfer.productId,
          productName: planToTransfer.productName,
          scheduledDate: newDate || planToTransfer.date,
          scheduledTime: planToTransfer.startTime || '08:00',
          mode,
          createdBy: mode === 'planning' ? 'System Planner' : 'Production Controller'
        });
      }

      const updatedPlans = prevPlans.map(plan => {
        if (plan.jobId === jobId && plan.machineId === fromMachineId) {
          transferSuccess = true;
          return {
            ...plan,
            machineId: toMachineId,
            date: newDate || plan.date,
            // Clear scheduling when transferring to maintain flexibility
            startTime: '',
            endTime: ''
          };
        }
        return plan;
      });
      
      return updatedPlans;
    });
    
    return transferSuccess;
  }, []);

  const getPlansForMachine = useCallback((machineId: string): ProductionPlan[] => {
    return plans.filter(plan => plan.machineId === machineId);
  }, [plans]);

  const updatePlan = useCallback((planId: string, updates: Partial<ProductionPlan>) => {
    setPlans(prevPlans => 
      prevPlans.map(plan => 
        plan.id === planId ? { ...plan, ...updates } : plan
      )
    );
  }, []);

  const checkMoldChange = useCallback((productId: string, targetMachineId: string): boolean => {
    const requiredMold = productMoldMapping[productId];
    const currentMold = machineMoldAssignments[targetMachineId];
    return requiredMold && currentMold && requiredMold !== currentMold;
  }, []);

  const createMCR = useCallback((params: {
    fromMoldId: string;
    toMoldId: string;
    machineId: string;
    jobId?: string;
    productId: string;
    productName: string;
    scheduledDate: Date;
    scheduledTime: string;
    mode: 'planning' | 'production-control';
    createdBy: string;
  }): string => {
    const mcrId = `MCR-${Date.now()}-${params.machineId}`;
    const now = new Date();
    
    const newMCR: MoldChangeRequest = {
      id: mcrId,
      fromMoldId: params.fromMoldId,
      toMoldId: params.toMoldId,
      fromMoldName: moldInfo[params.fromMoldId]?.name || 'Unknown Mold',
      toMoldName: moldInfo[params.toMoldId]?.name || 'Unknown Mold',
      machineId: params.machineId,
      machineName: `Machine ${params.machineId}`,
      jobId: params.jobId,
      productId: params.productId,
      productName: params.productName,
      status: params.mode === 'planning' ? 'planned' : 'pending',
      estimatedTime: moldInfo[params.toMoldId]?.changeTime || 60,
      createdBy: params.createdBy,
      createdAt: now,
      scheduledDate: params.scheduledDate,
      scheduledTime: params.scheduledTime,
    };

    setMcrRequests(prev => [...prev, newMCR]);
    return mcrId;
  }, []);

  const getMCRsForMachine = useCallback((machineId: string): MoldChangeRequest[] => {
    return mcrRequests.filter(mcr => mcr.machineId === machineId);
  }, [mcrRequests]);

  const updateMCR = useCallback((mcrId: string, updates: Partial<MoldChangeRequest>) => {
    setMcrRequests(prevMCRs => 
      prevMCRs.map(mcr => 
        mcr.id === mcrId ? { ...mcr, ...updates } : mcr
      )
    );
  }, []);

  const convertPlannedMCRsToPending = useCallback(() => {
    setMcrRequests(prevMCRs => 
      prevMCRs.map(mcr => 
        mcr.status === 'planned' ? { ...mcr, status: 'pending' } : mcr
      )
    );
  }, []);

  return (
    <JobScheduleContext.Provider value={{
      plans,
      setPlans,
      mcrRequests,
      setMcrRequests,
      transferJobToMachine,
      getPlansForMachine,
      updatePlan,
      checkMoldChange,
      createMCR,
      getMCRsForMachine,
      updateMCR,
      convertPlannedMCRsToPending
    }}>
      {children}
    </JobScheduleContext.Provider>
  );
}

export function useJobSchedule() {
  const context = useContext(JobScheduleContext);
  if (context === undefined) {
    throw new Error('useJobSchedule must be used within a JobScheduleProvider');
  }
  return context;
}