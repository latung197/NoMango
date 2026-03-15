import React, { useState } from "react";
import {
  ArrowRightLeft,
  Plus,
  Filter,
  Eye,
  Check,
  X,
  Download,
  Search,
  Calendar,
  MoreVertical,
  FileText,
  BarChart3,
} from "lucide-react";
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
} from "./ui/card";
import { Button } from "./ui/button";
import { Badge } from "./ui/badge";
import { Input } from "./ui/input";
import { Label } from "./ui/label";
import { Textarea } from "./ui/textarea";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "./ui/select";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "./ui/table";
import {
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from "./ui/tabs";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "./ui/dialog";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "./ui/alert-dialog";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
} from "recharts";

// Mock data
const borrowRequests = [
  {
    id: "BR-2024-001",
    fromJob: "Job-A001",
    fromMachine: "Extruder-01",
    toJob: "Job-B002",
    toMachine: "Molding-03",
    material: "PP Resin Grade A",
    quantity: 500,
    unit: "kg",
    status: "pending",
    requestedBy: "John Smith",
    approvedBy: null,
    requestDate: "2024-08-30",
    approvalDate: null,
    reason: "Urgent production requirement for customer order",
    remarks: "Required for immediate processing",
  },
  {
    id: "BR-2024-002",
    fromJob: "Job-C003",
    fromMachine: "Injection-02",
    toJob: "Job-D004",
    toMachine: "Blow-01",
    material: "PE Granules",
    quantity: 200,
    unit: "kg",
    status: "approved",
    requestedBy: "Mary Johnson",
    approvedBy: "Supervisor Wang",
    requestDate: "2024-08-29",
    approvalDate: "2024-08-30",
    reason: "Machine breakdown in primary line",
    remarks: "Approved for 48 hours usage",
  },
  {
    id: "BR-2024-003",
    fromJob: "Job-E005",
    fromMachine: "Extruder-02",
    toJob: "Job-F006",
    toMachine: "Cutting-01",
    material: "PVC Compound",
    quantity: 150,
    unit: "kg",
    status: "rejected",
    requestedBy: "David Lee",
    approvedBy: "Manager Chen",
    requestDate: "2024-08-28",
    approvalDate: "2024-08-29",
    reason: "Quality specification requirements",
    remarks: "Rejected due to material grade mismatch",
  },
];

const materials = [
  {
    id: "MAT-001",
    name: "PP Resin Grade A",
    stock: 2500,
    unit: "kg",
  },
  {
    id: "MAT-002",
    name: "PE Granules",
    stock: 1800,
    unit: "kg",
  },
  {
    id: "MAT-003",
    name: "PVC Compound",
    stock: 3200,
    unit: "kg",
  },
  {
    id: "MAT-004",
    name: "ABS Pellets",
    stock: 900,
    unit: "kg",
  },
  {
    id: "MAT-005",
    name: "PS Granules",
    stock: 1200,
    unit: "kg",
  },
];

const jobs = [
  "Job-A001",
  "Job-B002",
  "Job-C003",
  "Job-D004",
  "Job-E005",
  "Job-F006",
];

const machines = [
  "Extruder-01",
  "Extruder-02",
  "Molding-03",
  "Injection-02",
  "Blow-01",
  "Cutting-01",
];

// Chart data
const borrowingFrequency = [
  { machine: "Extruder-01", requests: 45 },
  { machine: "Molding-03", requests: 38 },
  { machine: "Injection-02", requests: 32 },
  { machine: "Blow-01", requests: 28 },
  { machine: "Cutting-01", requests: 22 },
];

const materialUsage = [
  { name: "PP Resin", value: 35, color: "#3b82f6" },
  { name: "PE Granules", value: 25, color: "#10b981" },
  { name: "PVC Compound", value: 20, color: "#f59e0b" },
  { name: "ABS Pellets", value: 12, color: "#ef4444" },
  { name: "PS Granules", value: 8, color: "#8b5cf6" },
];

export function BorrowModule() {
  const [activeTab, setActiveTab] = useState("requests");
  const [showCreateForm, setShowCreateForm] = useState(false);
  const [filterStatus, setFilterStatus] = useState("all");
  const [filterJob, setFilterJob] = useState("all");
  const [filterMachine, setFilterMachine] = useState("all");
  const [searchTerm, setSearchTerm] = useState("");

  // Form states
  const [formData, setFormData] = useState({
    fromJob: "",
    fromMachine: "",
    toJob: "",
    toMachine: "",
    material: "",
    quantity: "",
    reason: "",
    remarks: "",
  });

  const getStatusBadge = (status: string) => {
    switch (status) {
      case "pending":
        return (
          <Badge className="bg-yellow-100 text-yellow-800 border-yellow-300">
            🟡 Pending
          </Badge>
        );
      case "approved":
        return (
          <Badge className="bg-green-100 text-green-800 border-green-300">
            🟢 Approved
          </Badge>
        );
      case "rejected":
        return (
          <Badge className="bg-red-100 text-red-800 border-red-300">
            🔴 Rejected
          </Badge>
        );
      default:
        return <Badge variant="secondary">{status}</Badge>;
    }
  };

  const filteredRequests = borrowRequests.filter((request) => {
    const matchesStatus =
      filterStatus === "all" || request.status === filterStatus;
    const matchesJob =
      filterJob === "all" ||
      request.fromJob === filterJob ||
      request.toJob === filterJob;
    const matchesMachine =
      filterMachine === "all" ||
      request.fromMachine === filterMachine ||
      request.toMachine === filterMachine;
    const matchesSearch =
      searchTerm === "" ||
      request.id
        .toLowerCase()
        .includes(searchTerm.toLowerCase()) ||
      request.material
        .toLowerCase()
        .includes(searchTerm.toLowerCase()) ||
      request.requestedBy
        .toLowerCase()
        .includes(searchTerm.toLowerCase());

    return (
      matchesStatus &&
      matchesJob &&
      matchesMachine &&
      matchesSearch
    );
  });

  const pendingRequests = borrowRequests.filter(
    (req) => req.status === "pending",
  );

  const handleCreateRequest = () => {
    // Handle form submission
    console.log("Creating borrow request:", formData);
    setShowCreateForm(false);
    // Reset form
    setFormData({
      fromJob: "",
      fromMachine: "",
      toJob: "",
      toMachine: "",
      material: "",
      quantity: "",
      reason: "",
      remarks: "",
    });
  };

  const handleApproveRequest = (
    requestId: string,
    reason: string,
  ) => {
    console.log(
      `Approving request ${requestId} with reason: ${reason}`,
    );
  };

  const handleRejectRequest = (
    requestId: string,
    reason: string,
  ) => {
    console.log(
      `Rejecting request ${requestId} with reason: ${reason}`,
    );
  };

  return (
    <div className="p-6 bg-slate-50 min-h-screen">
      <div className="max-w-7xl mx-auto space-y-6">
        {/* Header */}
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-semibold text-slate-900">
              Borrow Module
            </h1>
            <p className="text-sm text-slate-600">
              ငှားယူမှုစနစ် - Material Transfer & Borrowing
              Management
            </p>
          </div>
          <div className="flex gap-3">
            <Dialog
              open={showCreateForm}
              onOpenChange={setShowCreateForm}
            >
              <DialogTrigger asChild>
                <Button className="bg-blue-600 hover:bg-blue-700 text-white">
                  <Plus className="h-4 w-4 mr-2" />
                  Create Request
                </Button>
              </DialogTrigger>
              <DialogContent className="max-w-2xl">
                <DialogHeader>
                  <DialogTitle>
                    Create Borrow Request
                  </DialogTitle>
                </DialogHeader>
                <div className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <Label>From Job</Label>
                      <Select
                        value={formData.fromJob}
                        onValueChange={(value) =>
                          setFormData((prev) => ({
                            ...prev,
                            fromJob: value,
                          }))
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Select source job" />
                        </SelectTrigger>
                        <SelectContent>
                          {jobs.map((job) => (
                            <SelectItem key={job} value={job}>
                              {job}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                    <div>
                      <Label>From Machine</Label>
                      <Select
                        value={formData.fromMachine}
                        onValueChange={(value) =>
                          setFormData((prev) => ({
                            ...prev,
                            fromMachine: value,
                          }))
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Select source machine" />
                        </SelectTrigger>
                        <SelectContent>
                          {machines.map((machine) => (
                            <SelectItem
                              key={machine}
                              value={machine}
                            >
                              {machine}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                  </div>

                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <Label>To Job</Label>
                      <Select
                        value={formData.toJob}
                        onValueChange={(value) =>
                          setFormData((prev) => ({
                            ...prev,
                            toJob: value,
                          }))
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Select destination job" />
                        </SelectTrigger>
                        <SelectContent>
                          {jobs.map((job) => (
                            <SelectItem key={job} value={job}>
                              {job}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                    <div>
                      <Label>To Machine</Label>
                      <Select
                        value={formData.toMachine}
                        onValueChange={(value) =>
                          setFormData((prev) => ({
                            ...prev,
                            toMachine: value,
                          }))
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Select destination machine" />
                        </SelectTrigger>
                        <SelectContent>
                          {machines.map((machine) => (
                            <SelectItem
                              key={machine}
                              value={machine}
                            >
                              {machine}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                  </div>

                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <Label>Material</Label>
                      <Select
                        value={formData.material}
                        onValueChange={(value) =>
                          setFormData((prev) => ({
                            ...prev,
                            material: value,
                          }))
                        }
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Select material" />
                        </SelectTrigger>
                        <SelectContent>
                          {materials.map((material) => (
                            <SelectItem
                              key={material.id}
                              value={material.name}
                            >
                              {material.name} (Stock:{" "}
                              {material.stock} {material.unit})
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </div>
                    <div>
                      <Label>Quantity</Label>
                      <Input
                        type="number"
                        placeholder="Enter quantity"
                        value={formData.quantity}
                        onChange={(e) =>
                          setFormData((prev) => ({
                            ...prev,
                            quantity: e.target.value,
                          }))
                        }
                      />
                    </div>
                  </div>

                  <div>
                    <Label>Reason</Label>
                    <Textarea
                      placeholder="Enter reason for borrowing"
                      value={formData.reason}
                      onChange={(e) =>
                        setFormData((prev) => ({
                          ...prev,
                          reason: e.target.value,
                        }))
                      }
                    />
                  </div>

                  <div>
                    <Label>Remarks</Label>
                    <Textarea
                      placeholder="Additional remarks (optional)"
                      value={formData.remarks}
                      onChange={(e) =>
                        setFormData((prev) => ({
                          ...prev,
                          remarks: e.target.value,
                        }))
                      }
                    />
                  </div>

                  <div className="flex justify-end gap-2 pt-4">
                    <Button
                      variant="outline"
                      onClick={() => setShowCreateForm(false)}
                    >
                      Cancel
                    </Button>
                    <Button
                      onClick={handleCreateRequest}
                      className="bg-blue-600 hover:bg-blue-700"
                    >
                      Submit Request
                    </Button>
                  </div>
                </div>
              </DialogContent>
            </Dialog>
          </div>
        </div>

        <Tabs
          value={activeTab}
          onValueChange={setActiveTab}
          className="w-full"
        >
          <TabsList className="grid w-full grid-cols-4">
            <TabsTrigger value="requests">
              Borrow Requests
            </TabsTrigger>
            <TabsTrigger value="approval">
              Approval Queue
            </TabsTrigger>
            <TabsTrigger value="history">History</TabsTrigger>
            <TabsTrigger value="reports">Reports</TabsTrigger>
          </TabsList>

          <TabsContent value="requests" className="space-y-4">
            {/* Filters */}
            <Card>
              <CardContent className="p-4">
                <div className="flex flex-wrap gap-4 items-end">
                  <div className="flex-1 min-w-60">
                    <Label>Search</Label>
                    <div className="relative">
                      <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-slate-400" />
                      <Input
                        placeholder="Search by Request ID, Material, or Requester"
                        value={searchTerm}
                        onChange={(e) =>
                          setSearchTerm(e.target.value)
                        }
                        className="pl-10"
                      />
                    </div>
                  </div>
                  <div>
                    <Label>Status</Label>
                    <Select
                      value={filterStatus}
                      onValueChange={setFilterStatus}
                    >
                      <SelectTrigger className="w-40">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">
                          All Status
                        </SelectItem>
                        <SelectItem value="pending">
                          Pending
                        </SelectItem>
                        <SelectItem value="approved">
                          Approved
                        </SelectItem>
                        <SelectItem value="rejected">
                          Rejected
                        </SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                  <div>
                    <Label>Job</Label>
                    <Select
                      value={filterJob}
                      onValueChange={setFilterJob}
                    >
                      <SelectTrigger className="w-32">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">
                          All Jobs
                        </SelectItem>
                        {jobs.map((job) => (
                          <SelectItem key={job} value={job}>
                            {job}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  <div>
                    <Label>Machine</Label>
                    <Select
                      value={filterMachine}
                      onValueChange={setFilterMachine}
                    >
                      <SelectTrigger className="w-40">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">
                          All Machines
                        </SelectItem>
                        {machines.map((machine) => (
                          <SelectItem
                            key={machine}
                            value={machine}
                          >
                            {machine}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Requests Table */}
            <Card>
              <CardHeader>
                <CardTitle>
                  Borrow Requests ({filteredRequests.length})
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Request ID</TableHead>
                        <TableHead>From</TableHead>
                        <TableHead>To</TableHead>
                        <TableHead>Material</TableHead>
                        <TableHead>Quantity</TableHead>
                        <TableHead>Status</TableHead>
                        <TableHead>Requested By</TableHead>
                        <TableHead>Date</TableHead>
                        <TableHead>Actions</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {filteredRequests.map((request) => (
                        <TableRow key={request.id}>
                          <TableCell className="font-medium">
                            {request.id}
                          </TableCell>
                          <TableCell>
                            <div className="text-sm">
                              <div>{request.fromJob}</div>
                              <div className="text-slate-500">
                                {request.fromMachine}
                              </div>
                            </div>
                          </TableCell>
                          <TableCell>
                            <div className="text-sm">
                              <div>{request.toJob}</div>
                              <div className="text-slate-500">
                                {request.toMachine}
                              </div>
                            </div>
                          </TableCell>
                          <TableCell>
                            {request.material}
                          </TableCell>
                          <TableCell>
                            {request.quantity} {request.unit}
                          </TableCell>
                          <TableCell>
                            {getStatusBadge(request.status)}
                          </TableCell>
                          <TableCell>
                            {request.requestedBy}
                          </TableCell>
                          <TableCell>
                            {request.requestDate}
                          </TableCell>
                          <TableCell>
                            <div className="flex gap-1">
                              <Button variant="ghost" size="sm">
                                <Eye className="h-4 w-4" />
                              </Button>
                              {request.status === "pending" && (
                                <>
                                  <AlertDialog>
                                    <AlertDialogTrigger asChild>
                                      <Button
                                        variant="ghost"
                                        size="sm"
                                        className="text-green-600 hover:text-green-700"
                                      >
                                        <Check className="h-4 w-4" />
                                      </Button>
                                    </AlertDialogTrigger>
                                    <AlertDialogContent>
                                      <AlertDialogHeader>
                                        <AlertDialogTitle>
                                          Approve Request
                                        </AlertDialogTitle>
                                        <AlertDialogDescription>
                                          Are you sure you want
                                          to approve request{" "}
                                          {request.id}?
                                        </AlertDialogDescription>
                                      </AlertDialogHeader>
                                      <AlertDialogFooter>
                                        <AlertDialogCancel>
                                          Cancel
                                        </AlertDialogCancel>
                                        <AlertDialogAction
                                          onClick={() =>
                                            handleApproveRequest(
                                              request.id,
                                              "",
                                            )
                                          }
                                        >
                                          Approve
                                        </AlertDialogAction>
                                      </AlertDialogFooter>
                                    </AlertDialogContent>
                                  </AlertDialog>
                                  <AlertDialog>
                                    <AlertDialogTrigger asChild>
                                      <Button
                                        variant="ghost"
                                        size="sm"
                                        className="text-red-600 hover:text-red-700"
                                      >
                                        <X className="h-4 w-4" />
                                      </Button>
                                    </AlertDialogTrigger>
                                    <AlertDialogContent>
                                      <AlertDialogHeader>
                                        <AlertDialogTitle>
                                          Reject Request
                                        </AlertDialogTitle>
                                        <AlertDialogDescription>
                                          Are you sure you want
                                          to reject request{" "}
                                          {request.id}?
                                        </AlertDialogDescription>
                                      </AlertDialogHeader>
                                      <AlertDialogFooter>
                                        <AlertDialogCancel>
                                          Cancel
                                        </AlertDialogCancel>
                                        <AlertDialogAction
                                          onClick={() =>
                                            handleRejectRequest(
                                              request.id,
                                              "",
                                            )
                                          }
                                          className="bg-red-600 hover:bg-red-700"
                                        >
                                          Reject
                                        </AlertDialogAction>
                                      </AlertDialogFooter>
                                    </AlertDialogContent>
                                  </AlertDialog>
                                </>
                              )}
                            </div>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="approval" className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle>
                  Pending Approvals ({pendingRequests.length})
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {pendingRequests.map((request) => (
                    <div
                      key={request.id}
                      className="border rounded-lg p-4 bg-slate-50"
                    >
                      <div className="flex justify-between items-start">
                        <div className="grid grid-cols-3 gap-4 flex-1">
                          <div>
                            <Label className="text-sm font-medium">
                              Request Details
                            </Label>
                            <div className="mt-1">
                              <div className="font-medium">
                                {request.id}
                              </div>
                              <div className="text-sm text-slate-600">
                                By {request.requestedBy}
                              </div>
                              <div className="text-sm text-slate-600">
                                {request.requestDate}
                              </div>
                            </div>
                          </div>
                          <div>
                            <Label className="text-sm font-medium">
                              Transfer Details
                            </Label>
                            <div className="mt-1 text-sm">
                              <div>
                                From: {request.fromJob} (
                                {request.fromMachine})
                              </div>
                              <div>
                                To: {request.toJob} (
                                {request.toMachine})
                              </div>
                              <div>
                                Material: {request.material}
                              </div>
                              <div>
                                Quantity: {request.quantity}{" "}
                                {request.unit}
                              </div>
                            </div>
                          </div>
                          <div>
                            <Label className="text-sm font-medium">
                              Reason
                            </Label>
                            <div className="mt-1 text-sm text-slate-600">
                              {request.reason}
                            </div>
                          </div>
                        </div>
                        <div className="flex gap-2 ml-4">
                          <Button
                            size="sm"
                            className="bg-green-600 hover:bg-green-700 text-white"
                            onClick={() =>
                              handleApproveRequest(
                                request.id,
                                "",
                              )
                            }
                          >
                            <Check className="h-4 w-4 mr-1" />
                            Approve
                          </Button>
                          <Button
                            size="sm"
                            variant="outline"
                            className="text-red-600 hover:text-red-700 border-red-200 hover:border-red-300"
                            onClick={() =>
                              handleRejectRequest(
                                request.id,
                                "",
                              )
                            }
                          >
                            <X className="h-4 w-4 mr-1" />
                            Reject
                          </Button>
                        </div>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="history" className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle>Borrowing History</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Request ID</TableHead>
                        <TableHead>From → To</TableHead>
                        <TableHead>Material</TableHead>
                        <TableHead>Quantity</TableHead>
                        <TableHead>Status</TableHead>
                        <TableHead>Requester</TableHead>
                        <TableHead>Approver</TableHead>
                        <TableHead>Dates</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {borrowRequests.map((request) => (
                        <TableRow key={request.id}>
                          <TableCell className="font-medium">
                            {request.id}
                          </TableCell>
                          <TableCell>
                            <div className="text-sm">
                              <div>
                                {request.fromJob} (
                                {request.fromMachine})
                              </div>
                              <div className="text-slate-400">
                                ↓
                              </div>
                              <div>
                                {request.toJob} (
                                {request.toMachine})
                              </div>
                            </div>
                          </TableCell>
                          <TableCell>
                            {request.material}
                          </TableCell>
                          <TableCell>
                            {request.quantity} {request.unit}
                          </TableCell>
                          <TableCell>
                            {getStatusBadge(request.status)}
                          </TableCell>
                          <TableCell>
                            {request.requestedBy}
                          </TableCell>
                          <TableCell>
                            {request.approvedBy || "-"}
                          </TableCell>
                          <TableCell>
                            <div className="text-sm">
                              <div>
                                Req: {request.requestDate}
                              </div>
                              {request.approvalDate && (
                                <div>
                                  App: {request.approvalDate}
                                </div>
                              )}
                            </div>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </CardContent>
            </Card>
          </TabsContent>

          <TabsContent value="reports" className="space-y-6">
            {/* Summary Cards */}
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center gap-2">
                    <div className="p-2 bg-blue-100 rounded-lg">
                      <ArrowRightLeft className="h-5 w-5 text-blue-600" />
                    </div>
                    <div>
                      <div className="text-2xl font-semibold">
                        23
                      </div>
                      <div className="text-sm text-slate-600">
                        Total Requests
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center gap-2">
                    <div className="p-2 bg-yellow-100 rounded-lg">
                      <Calendar className="h-5 w-5 text-yellow-600" />
                    </div>
                    <div>
                      <div className="text-2xl font-semibold">
                        5
                      </div>
                      <div className="text-sm text-slate-600">
                        Pending
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center gap-2">
                    <div className="p-2 bg-green-100 rounded-lg">
                      <Check className="h-5 w-5 text-green-600" />
                    </div>
                    <div>
                      <div className="text-2xl font-semibold">
                        15
                      </div>
                      <div className="text-sm text-slate-600">
                        Approved
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
              <Card>
                <CardContent className="p-4">
                  <div className="flex items-center gap-2">
                    <div className="p-2 bg-red-100 rounded-lg">
                      <X className="h-5 w-5 text-red-600" />
                    </div>
                    <div>
                      <div className="text-2xl font-semibold">
                        3
                      </div>
                      <div className="text-sm text-slate-600">
                        Rejected
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>

            {/* Charts */}
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
              <Card>
                <CardHeader>
                  <CardTitle>
                    Borrowing Frequency by Machine
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <ResponsiveContainer
                    width="100%"
                    height={300}
                  >
                    <BarChart data={borrowingFrequency}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="machine" />
                      <YAxis />
                      <Tooltip />
                      <Bar dataKey="requests" fill="#3b82f6" />
                    </BarChart>
                  </ResponsiveContainer>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Top Borrowed Materials</CardTitle>
                </CardHeader>
                <CardContent>
                  <ResponsiveContainer
                    width="100%"
                    height={300}
                  >
                    <PieChart>
                      <Pie
                        data={materialUsage}
                        cx="50%"
                        cy="50%"
                        outerRadius={80}
                        dataKey="value"
                        label={({ name, value }) =>
                          `${name}: ${value}%`
                        }
                      >
                        {materialUsage.map((entry, index) => (
                          <Cell
                            key={`cell-${index}`}
                            fill={entry.color}
                          />
                        ))}
                      </Pie>
                      <Tooltip />
                    </PieChart>
                  </ResponsiveContainer>
                </CardContent>
              </Card>
            </div>

            {/* Export Section */}
            <Card>
              <CardHeader>
                <CardTitle>Export Reports</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="flex gap-4">
                  <Button variant="outline">
                    <Download className="h-4 w-4 mr-2" />
                    Export CSV
                  </Button>
                  <Button variant="outline">
                    <Download className="h-4 w-4 mr-2" />
                    Export Excel
                  </Button>
                  <Button variant="outline">
                    <FileText className="h-4 w-4 mr-2" />
                    Export PDF
                  </Button>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
        </Tabs>
      </div>
    </div>
  );
}