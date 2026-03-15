export interface ProductionReport {
  id: string;
  date: string;
  shift: string;
  machine: string;
  product: string;
  targetQuantity: number;
  actualQuantity: number;
  efficiency: number;
  downtime: number;
  operator: string;
  qcStatus: string;
}

export interface QCReport {
  id: string;
  date: string;
  product: string;
  batch: string;
  inspector: string;
  tested: number;
  passed: number;
  failed: number;
  defectType: string;
  status: string;
}

export interface InventoryReport {
  id: string;
  date: string;
  warehouse: string;
  item: string;
  category: string;
  openingStock: number;
  received: number;
  issued: number;
  closingStock: number;
  value: number;
}

export const productionReports: ProductionReport[] = [
  {
    id: 'PR001',
    date: '2024-12-27',
    shift: 'Morning',
    machine: 'Injection Molding #1',
    product: 'Plastic Container Type A',
    targetQuantity: 500,
    actualQuantity: 475,
    efficiency: 95,
    downtime: 30,
    operator: 'John Doe',
    qcStatus: 'Passed'
  },
  {
    id: 'PR002',
    date: '2024-12-27',
    shift: 'Evening',
    machine: 'Injection Molding #2',
    product: 'Plastic Cup Type B',
    targetQuantity: 300,
    actualQuantity: 280,
    efficiency: 93,
    downtime: 45,
    operator: 'Jane Smith',
    qcStatus: 'Passed'
  },
  {
    id: 'PR003',
    date: '2024-12-26',
    shift: 'Morning',
    machine: 'Extrusion Line #1',
    product: 'Plastic Bottle Type C',
    targetQuantity: 200,
    actualQuantity: 180,
    efficiency: 90,
    downtime: 60,
    operator: 'Mike Johnson',
    qcStatus: 'Failed'
  }
];

export const qcReports: QCReport[] = [
  {
    id: 'QC001',
    date: '2024-12-27',
    product: 'Plastic Container Type A',
    batch: 'BATCH001',
    inspector: 'Sarah Wilson',
    tested: 50,
    passed: 48,
    failed: 2,
    defectType: 'Surface defects',
    status: 'Completed'
  },
  {
    id: 'QC002',
    date: '2024-12-27',
    product: 'Plastic Cup Type B',
    batch: 'BATCH002',
    inspector: 'David Brown',
    tested: 30,
    passed: 30,
    failed: 0,
    defectType: 'None',
    status: 'Completed'
  },
  {
    id: 'QC003',
    date: '2024-12-26',
    product: 'Plastic Bottle Type C',
    batch: 'BATCH003',
    inspector: 'Lisa Anderson',
    tested: 25,
    passed: 20,
    failed: 5,
    defectType: 'Dimension issues',
    status: 'Completed'
  }
];

export const inventoryReports: InventoryReport[] = [
  {
    id: 'INV001',
    date: '2024-12-27',
    warehouse: 'Raw Material WH',
    item: 'Plastic Pellets Type A',
    category: 'Raw Material',
    openingStock: 2800,
    received: 500,
    issued: 800,
    closingStock: 2500,
    value: 125000
  },
  {
    id: 'INV002',
    date: '2024-12-27',
    warehouse: 'Finished Goods WH',
    item: 'Plastic Containers Type A',
    category: 'Finished Goods',
    openingStock: 1800,
    received: 475,
    issued: 275,
    closingStock: 2000,
    value: 400000
  },
  {
    id: 'INV003',
    date: '2024-12-27',
    warehouse: 'Packing WH',
    item: 'Cardboard Boxes',
    category: 'Packing Materials',
    openingStock: 600,
    received: 200,
    issued: 300,
    closingStock: 500,
    value: 25000
  }
];