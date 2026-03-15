import React from 'react';

// Import individual registration components
import { MachineRegistration } from './master-data/MachineRegistration';
import { UserRegistration } from './master-data/UserRegistration';
import { MoldToolingRegistration } from './master-data/MoldToolingRegistration';
import { RawMaterialRegistration } from './master-data/RawMaterialRegistration';
import { ReasonCodeRegistration } from './master-data/ReasonCodeRegistration';
import { DeviceRegistration } from './master-data/DeviceRegistration';
import { CustomerHQRegistration } from './master-data/CustomerHQRegistration';
import { UnitMeasurementSetup } from './master-data/UnitMeasurementSetup';
import { RoleMatrixManagement } from './master-data/RoleMatrixManagement';

interface MasterDataModuleProps {
  currentPage?: string;
  onPageChange?: (page: string) => void;
}

export function MasterDataModule({ currentPage, onPageChange }: MasterDataModuleProps) {
  const renderContent = () => {
    switch (currentPage) {
      case 'machines':
        return <MachineRegistration />;
      case 'users':
        return <UserRegistration />;
      case 'molds-tooling':
        return <MoldToolingRegistration />;
      case 'raw-materials':
        return <RawMaterialRegistration />;
      case 'reason-codes':
        return <ReasonCodeRegistration />;
      case 'devices':
        return <DeviceRegistration />;
      case 'customers-hq':
        return <CustomerHQRegistration />;
      case 'units-measure':
        return <UnitMeasurementSetup />;
      case 'role-matrix':
        return <RoleMatrixManagement />;
      default:
        return <MachineRegistration />;
    }
  };

  return (
    <div className="bg-slate-50 min-h-screen">
      {renderContent()}
    </div>
  );
}