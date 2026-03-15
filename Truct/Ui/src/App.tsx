import React, { useState, Suspense } from 'react';
import { Layout } from './components/Layout';

// Lazy load components to improve performance
const MachineOverviewDashboard = React.lazy(() => import('./components/MachineOverviewDashboard'));
const MachineCalendarView = React.lazy(() => import('./components/MachineCalendarView'));
const ExecutiveDashboard = React.lazy(() => import('./components/ExecutiveDashboard'));

const ProductMaster = React.lazy(() => import('./components/ProductMaster').then(module => ({ default: module.ProductMaster })));
const ProductionPlanning = React.lazy(() => import('./components/ProductionPlanning').then(module => ({ default: module.ProductionPlanning })));

const PlanningDashboard = React.lazy(() => import('./components/PlanningDashboard'));
const PlanningReports = React.lazy(() => import('./components/PlanningReports').then(module => ({ default: module.PlanningReports })));
const JobPlanningScheduleEnhanced = React.lazy(() => import('./components/JobPlanningScheduleEnhanced').then(module => ({ default: module.JobPlanningSchedule })));
const LiveSchedule = React.lazy(() => import('./components/LiveSchedule'));

const ProductionControl = React.lazy(() => import('./components/ProductionControl').then(module => ({ default: module.ProductionControl })));
const QualityControl = React.lazy(() => import('./components/QualityControl').then(module => ({ default: module.QualityControl })));
const EngineeringModule = React.lazy(() => import('./components/EngineeringModule').then(module => ({ default: module.EngineeringModule })));
const EngineeringMobilePWA = React.lazy(() => import('./components/EngineeringMobilePWA').then(module => ({ default: module.EngineeringMobilePWA })));
const InventoryModule = React.lazy(() => import('./components/InventoryModule').then(module => ({ default: module.InventoryModule })));
const Inventory = React.lazy(() => import('./components/Inventory').then(module => ({ default: module.Inventory })));
const HRModule = React.lazy(() => import('./components/HRModule').then(module => ({ default: module.HRModule })));
const MasterDataModule = React.lazy(() => import('./components/MasterDataModule').then(module => ({ default: module.MasterDataModule })));
const MasterData = React.lazy(() => import('./components/MasterData').then(module => ({ default: module.MasterData })));
const ProductionModule = React.lazy(() => import('./components/ProductionModule').then(module => ({ default: module.ProductionModule })));
const RawMaterialProductMapping = React.lazy(() => import('./components/RawMaterialProductMapping').then(module => ({ default: module.RawMaterialProductMapping })));
const RawMaterialRegistration = React.lazy(() => import('./components/RawMaterialRegistration').then(module => ({ default: module.RawMaterialRegistration })));
const RawMaterialWarehouse = React.lazy(() => import('./components/RawMaterialWarehouse').then(module => ({ default: module.RawMaterialWarehouse })));
const CreatePlanForm = React.lazy(() => import('./components/CreatePlanForm').then(module => ({ default: module.CreatePlanForm })));
const ProductColorMapping = React.lazy(() => import('./components/ProductColorMapping').then(module => ({ default: module.ProductColorMapping })));
const BorrowModule = React.lazy(() => import('./components/BorrowModule').then(module => ({ default: module.BorrowModule })));
const AdvancedReporting = React.lazy(() => import('./components/AdvancedReporting').then(module => ({ default: module.AdvancedReporting })));
const LogisticControl = React.lazy(() => import('./components/LogisticControl').then(module => ({ default: module.LogisticControl })));
const ProductMasterEnhanced = React.lazy(() => import('./components/ProductMasterEnhanced').then(module => ({ default: module.ProductMaster })));
// Import JobScheduleProvider directly since it's needed for context
import { JobScheduleProvider } from './components/JobScheduleContext';

export default function App() {
  const [currentPage, setCurrentPage] = useState('dashboard');
  const [selectedMachineId, setSelectedMachineId] = useState<string | null>(null);

  const renderCurrentPage = () => {
    const LoadingSpinner = () => (
      <div className="flex items-center justify-center min-h-[400px]">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-500"></div>
        <span className="ml-2 text-slate-600">Loading...</span>
      </div>
    );

    switch (currentPage) {
      case 'dashboard':
      case 'executive-dashboard':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <ExecutiveDashboard />
          </Suspense>
        );

      case 'machine-calendar':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <JobScheduleProvider>
              <MachineCalendarView 
                machineId={selectedMachineId || 'IM-001'}
                onBack={() => {
                  setCurrentPage('planning-calendar');
                  setSelectedMachineId(null);
                }}
                onOpenInNewTab={() => {
                  window.open(`#machine-calendar/${selectedMachineId}`, '_blank');
                }}
              />
            </JobScheduleProvider>
          </Suspense>
        );
      case 'production-dashboard':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <LiveSchedule currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );

      case 'machine-production-dashboard':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <JobScheduleProvider>
              <MachineCalendarView 
                machineId={selectedMachineId || 'IM-001'}
                onBack={() => {
                  setCurrentPage('production-dashboard');
                  setSelectedMachineId(null);
                }}
                onOpenInNewTab={() => {
                  window.open(`#machine-production-dashboard/${selectedMachineId}`, '_blank');
                }}
                mode="production-control"
                breadcrumbContext="Production Control > Production Dashboard"
              />
            </JobScheduleProvider>
          </Suspense>
        );

      case 'planning-dashboard':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <PlanningDashboard 
              currentPage={currentPage} 
              onPageChange={setCurrentPage}
              onMachineSelect={(machineId) => {
                setSelectedMachineId(machineId);
                setCurrentPage('planning-machine-calendar');
              }}
            />
          </Suspense>
        );

      case 'planning-machine-calendar':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <JobScheduleProvider>
              <MachineCalendarView 
                machineId={selectedMachineId || 'IM-001'}
                onBack={() => {
                  setCurrentPage('planning-dashboard');
                  setSelectedMachineId(null);
                }}
                onOpenInNewTab={() => {
                  window.open(`#planning-machine-calendar/${selectedMachineId}`, '_blank');
                }}
                mode="planning"
                breadcrumbContext="Planning Control > Planning Dashboard"
              />
            </JobScheduleProvider>
          </Suspense>
        );

      case 'product-master':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <ProductMasterEnhanced />
          </Suspense>
        );
      case 'production-planning':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <ProductionPlanning />
          </Suspense>
        );


      case 'job-planning-schedule':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <JobPlanningScheduleEnhanced />
          </Suspense>
        );

      case 'planning-reports':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <PlanningReports />
          </Suspense>
        );
      case 'production-control':
      case 'demand-authorization':
      case 'material-control':
      case 'job-monitoring':
      case 'mold-change-request':
      case 'finished-goods-transfer':
      case 'waste-management':
      case 'split-job-management':
      case 'reject-management':
      case 'cut-glue-residue':
      case 'job-tracker':
      case 'mold-change-requests':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <ProductionControl currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );
      case 'qc-dashboard':
      case 'raw-material-qc':
      case 'in-process-qc':
      case 'finished-goods-qc':
      case 'qc-reports':
      case 'qc-settings':
      case 'qc-entry-form':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <QualityControl currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );
      case 'engineering-dashboard':
      case 'plastic-engineer':
      case 'mold-engineer':
      case 'maintenance-engineer':
      case 'technician-head':
      case 'engineering-reports':
      case 'mold-change-tasks':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <JobScheduleProvider>
              <EngineeringModule currentPage={currentPage} onPageChange={setCurrentPage} />
            </JobScheduleProvider>
          </Suspense>
        );
      case 'engineering-module':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <EngineeringModule />
          </Suspense>
        );
      case 'engineering-mobile-pwa':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <EngineeringMobilePWA />
          </Suspense>
        );
      case 'warehouse-registration':
      case 'warehouse-routes':
      case 'warehouse-dashboard':
      case 'finished-goods-intake':
      case 'borrow-center':
      case 'inventory-reports-main':
      case 'user-permissions-matrix':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <InventoryModule currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );
      case 'raw-material-registration':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <RawMaterialRegistration currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );
      case 'raw-material-warehouse':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <RawMaterialWarehouse />
          </Suspense>
        );
      case 'warehouse-management':
      case 'stock-transactions':
      case 'finished-goods-receiving':
      case 'waste-reject-handling':
      case 'inventory-reports':
      case 'user-permissions':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <Inventory currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );

      case 'staff-registration':
      case 'department-registration':
      case 'shift-flexible-setup':
      case 'shift-work-management':
      case 'attendance-record':
      case 'hr-reports':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <HRModule currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );
      case 'machines':
      case 'users':
      case 'molds-tooling':
      case 'reason-codes':
      case 'devices':
      case 'customers-hq':
      case 'units-measure':
      case 'role-matrix':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <MasterDataModule currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );
      case 'raw-material-product-mapping':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <RawMaterialProductMapping currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );
      case 'users-roles':
      case 'products':
      case 'raw-materials':
      case 'warehouses':
      case 'shifts-schedules':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <MasterData currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );
      case 'operator-kiosk':
      case 'glue-fill-operator':
      case 'supervisor-screen':
      case 'live-monitoring':
      case 'operator-dashboard':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <ProductionModule currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );

      case 'production-plan':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <CreatePlanForm />
          </Suspense>
        );
      case 'product-color-mapping':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <ProductColorMapping />
          </Suspense>
        );
      case 'borrow-module':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <BorrowModule />
          </Suspense>
        );
      case 'production-reports':
      case 'daily-production-summary':
      case 'machine-output-log':
      case 'qr-code-history':
      case 'defect-rejection-report':
      case 'operator-production-report':
      case 'operator-shift-assignment':
      case 'operator-productivity-ranking':
      case 'qc-reports-main':
      case 'qc-inspection-summary':
      case 'defect-analysis-report':
      case 'batch-quality-log':
      case 'inventory-reports-main':
      case 'raw-material-inout':
      case 'finished-goods-movement':
      case 'current-stock-summary':
      case 'leftover-raw-material':
      case 'expired-unused-material':
      case 'reserved-available-stock':
      case 'borrowing-reallocation-history':
      case 'fifo-batch-lot-report':
      case 'dispatch-tracking-report':
      case 'hr-reports-main':
      case 'daily-operator-assignment':
      case 'working-hours-rfid-logs':
      case 'operator-productivity-report-hr':
      case 'operator-downtime-association':
      case 'engineering-reports-main':
      case 'machine-efficiency-analysis':
      case 'process-optimization-report':
      case 'technical-documentation':
      case 'maintenance-reports':
      case 'machine-downtime-log':
      case 'preventive-maintenance-schedule':
      case 'spare-parts-usage-summary':
      case 'machine-health-history':
      case 'hq-sales-reports':
      case 'finished-goods-sent-hq':
      case 'dispatch-tracking-report-sales':
      case 'glue-pellet-sales-report':
      case 'executive-analytics-reports':
      case 'production-output-chart':
      case 'machine-downtime-analysis':
      case 'operator-performance-leaderboard':
      case 'alerts-notifications-report':
      case 'advanced-reporting':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <AdvancedReporting currentPage={currentPage} onPageChange={setCurrentPage} />
          </Suspense>
        );

      case 'logistic-control':
        return (
          <Suspense fallback={<LoadingSpinner />}>
            <LogisticControl />
          </Suspense>
        );
      default:
        return (
          <div className="p-6 bg-slate-50 min-h-screen">
            <div className="max-w-4xl mx-auto">
              <div className="bg-white rounded-lg shadow-sm p-8 text-center">
                <h2 className="text-xl font-semibold text-slate-900 mb-2">
                  {getPageTitle(currentPage)}
                </h2>
                <p className="text-slate-600 mb-4">
                  This page is under development
                </p>
                <p className="text-sm text-slate-500">
                  ဤစာမျက်နှာကို ဖွံ့ဖြိုးတိုးတက်နေဆဲဖြစ်သည်
                </p>
              </div>
            </div>
          </div>
        );
    }
  };

  const getPageTitle = (pageId: string) => {
    const pageTitles: Record<string, string> = {
      'product-master': 'Product Master',
      'production-planning': 'Production Planning',

      'planning-dashboard': 'Planning Dashboard',
      'planning-machine-calendar': 'Planning Machine Calendar',
      'job-planning-schedule': 'Job Planning Schedule',
      'production-dashboard': 'Production Dashboard',
      'machine-production-dashboard': 'Machine Production Dashboard',

      'production-control': 'Production Control',
      'product-color-mapping': 'Product-Color Mapping',
      'production-plan': 'Create Production Plan',

      'planning-reports': 'Planning Reports',
      'live-monitoring': 'Live Monitoring',
      'qc-dashboard': 'QC Dashboard',
      'raw-material-qc': 'Raw Material QC (After Issue)',
      'in-process-qc': 'In-Process QC',
      'finished-goods-qc': 'Finished Goods QC (Before QR Print)',
      'qc-reports': 'QC Reports',
      'qc-settings': 'QC Settings',
      'qc-entry-form': 'QC Entry Form',
      'engineering-dashboard': 'Engineering Dashboard',
      'plastic-engineer': 'Plastic Technician',
      'mold-engineer': 'Mold Technician',
      'maintenance-engineer': 'Maintenance Engineer (Maintenance Staff View)',
      'technician-head': 'Technician Head (Verification/Approval View)',
      'engineering-reports': 'Engineering Reports',
      'warehouse-management': 'Warehouse Management',

      'finished-goods-receiving': 'Finished Goods Receiving',
      'waste-reject-handling': 'Waste & Reject Handling',

      'inventory-reports': 'Inventory Reports',
      'user-permissions': 'User Permissions Matrix',

      'operator-dashboard': 'Operator Dashboard',
      'shift-assignment': 'Shift & Assignment Management',
      'attendance-logs': 'Attendance & Working Hour Logs',
      'productivity-reports': 'Operator Productivity Reports',
      'role-access-control': 'Role & Access Control',
      'operator-profiles': 'Operator Profiles',
      'qualifications': 'Qualifications',
      'shift-roster': 'Shift & Roster',
      'working-hours': 'Working Hours',
      'hr-reports': 'HR Reports',
      'machines': 'Machines',
      'users-roles': 'Users & Roles',
      'products': 'Products',
      'raw-materials': 'Raw Materials',
      'molds-tooling': 'Molds & Tooling',
      'warehouses': 'Warehouses',
      'shifts-schedules': 'Shifts & Schedules',
      'reason-codes': 'Reason Codes',
      'devices': 'Devices',
      'customers-hq': 'Customers & HQ',
      'units-measure': 'Units of Measure',
      'borrow-module': 'Borrow Module',
      'demand-authorization': 'Demand Authorization',
      'material-control': 'Raw Material Control',
      'job-monitoring': 'Job Monitoring',
      'mold-change-request': 'Mold Change Request',
      'finished-goods-transfer': 'Finished Goods Transfer',
      'waste-management': 'Waste Management',
      'split-job-management': 'Split Job Management',
      'reject-management': 'Reject Management',
      'cut-glue-residue': 'Crushing, Flash, Nozzle Waste Management',
      'job-tracker': 'Job Tracker',
      'advanced-reporting': 'Advanced Reporting',
      'role-matrix': 'Role Matrix',
      'raw-material-registration': 'Raw Material Registration',
      'raw-material-warehouse': 'Raw Material Warehouse',
      'raw-material-product-mapping': 'Raw Material ↔ Product Mapping',
      'engineering-module': 'Engineering Module',
      'engineering-mobile-pwa': 'Engineering Mobile PWA',
      'glue-fill-operator': 'Glue Fill Operator',
      'logistic-control': 'Logistic Control'
    };
    return pageTitles[pageId] || 'Unknown Page';
  };

  return (
    <Layout 
      currentPage={currentPage} 
      onPageChange={setCurrentPage}
    >
      {renderCurrentPage()}
    </Layout>
  );
}