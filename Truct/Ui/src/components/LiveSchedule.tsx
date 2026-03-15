import React, { useState } from 'react';
import MachineOverviewDashboard from './MachineOverviewDashboard';
import { MachineCalendarView } from './MachineCalendarView';
import { JobScheduleProvider } from './JobScheduleContext';

interface LiveScheduleProps {
  currentPage?: string;
  onPageChange?: (page: string) => void;
}

function LiveSchedule({ currentPage, onPageChange }: LiveScheduleProps) {
  const [selectedMachineId, setSelectedMachineId] = useState<string | null>(null);
  const [viewMode, setViewMode] = useState<'overview' | 'machine-schedule'>('overview');

  const handleMachineSelect = (machineId: string) => {
    setSelectedMachineId(machineId);
    setViewMode('machine-schedule');
  };

  const handleBackToOverview = () => {
    setSelectedMachineId(null);
    setViewMode('overview');
  };

  if (viewMode === 'machine-schedule' && selectedMachineId) {
    return (
      <JobScheduleProvider>
        <MachineCalendarView 
          machineId={selectedMachineId}
          onBack={handleBackToOverview}
          onOpenInNewTab={() => {
            window.open(`#production-dashboard-machine/${selectedMachineId}`, '_blank');
          }}
          mode="production-control" // This prop will distinguish it from planning mode
          breadcrumbContext="Production Control > Production Dashboard"
        />
      </JobScheduleProvider>
    );
  }

  return (
    <JobScheduleProvider>
      <MachineOverviewDashboard 
        onMachineSelect={handleMachineSelect}
      />
    </JobScheduleProvider>
  );
}

export default LiveSchedule;