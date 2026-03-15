import React, { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';
import { Button } from './ui/button';
import { Badge } from './ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from './ui/table';
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from './ui/dialog';
import { Textarea } from './ui/textarea';
import { Label } from './ui/label';
import { Input } from './ui/input';
import { Tabs, TabsContent, TabsList, TabsTrigger } from './ui/tabs';
import { 
  Settings, 
  Clock, 
  CheckCircle, 
  XCircle, 
  Play, 
  Pause, 
  AlertTriangle,
  Eye,
  ThumbsUp,
  ThumbsDown,
  Timer,
  User,
  Calendar,
  Package,
  Factory,
  Wrench
} from 'lucide-react';
import { useJobSchedule, MoldChangeRequest } from './JobScheduleContext';
import { toast } from 'sonner';
import { format } from 'date-fns';

interface MoldChangeRequestsProps {
  userRole: 'production-control' | 'mold-technician';
}

export function MoldChangeRequests({ userRole }: MoldChangeRequestsProps) {
  const { mcrRequests, updateMCR } = useJobSchedule();
  const [selectedMCR, setSelectedMCR] = useState<MoldChangeRequest | null>(null);
  const [showDetailDialog, setShowDetailDialog] = useState(false);
  const [showApprovalDialog, setShowApprovalDialog] = useState(false);
  const [showTimeLogDialog, setShowTimeLogDialog] = useState(false);
  const [rejectionReason, setRejectionReason] = useState('');
  const [actualTime, setActualTime] = useState('');
  const [notes, setNotes] = useState('');

  // Filter MCRs based on user role
  const filteredMCRs = mcrRequests.filter(mcr => {
    if (userRole === 'production-control') {
      return ['pending', 'approved', 'rejected'].includes(mcr.status);
    } else if (userRole === 'mold-technician') {
      return ['approved', 'in-progress', 'completed'].includes(mcr.status);
    }
    return false;
  });

  const getStatusConfig = (status: string) => {
    const configs = {
      'planned': { color: 'bg-gray-100 text-gray-800', icon: '⏳', text: 'Planned' },
      'pending': { color: 'bg-blue-100 text-blue-800', icon: '⏳', text: 'Pending' },
      'approved': { color: 'bg-green-100 text-green-800', icon: '✅', text: 'Approved' },
      'rejected': { color: 'bg-red-100 text-red-800', icon: '❌', text: 'Rejected' },
      'in-progress': { color: 'bg-orange-100 text-orange-800', icon: '🔧', text: 'In Progress' },
      'completed': { color: 'bg-emerald-100 text-emerald-800', icon: '✨', text: 'Completed' }
    };
    return configs[status as keyof typeof configs] || configs.pending;
  };

  const handleApprove = (mcr: MoldChangeRequest) => {
    updateMCR(mcr.id, {
      status: 'approved',
      approvedBy: 'Production Controller',
      approvedAt: new Date()
    });
    toast.success('MCR Approved', {
      description: `Mold change request for ${mcr.machineName} has been approved.`
    });
    setShowApprovalDialog(false);
  };

  const handleReject = (mcr: MoldChangeRequest) => {
    if (!rejectionReason.trim()) {
      toast.error('Rejection reason is required');
      return;
    }
    
    updateMCR(mcr.id, {
      status: 'rejected',
      rejectedBy: 'Production Controller',
      rejectedAt: new Date(),
      rejectionReason
    });
    toast.success('MCR Rejected', {
      description: `Mold change request for ${mcr.machineName} has been rejected.`
    });
    setShowApprovalDialog(false);
    setRejectionReason('');
  };

  const handleAccept = (mcr: MoldChangeRequest) => {
    updateMCR(mcr.id, {
      status: 'in-progress',
      acceptedBy: 'Mold Technician',
      acceptedAt: new Date(),
      startedBy: 'Mold Technician',
      startedAt: new Date()
    });
    toast.success('MCR Started', {
      description: `Mold change for ${mcr.machineName} is now in progress.`
    });
  };

  const handleComplete = (mcr: MoldChangeRequest) => {
    if (!actualTime.trim()) {
      toast.error('Actual time is required');
      return;
    }
    
    updateMCR(mcr.id, {
      status: 'completed',
      completedBy: 'Mold Technician',
      completedAt: new Date(),
      actualTime: parseInt(actualTime),
      notes
    });
    toast.success('MCR Completed', {
      description: `Mold change for ${mcr.machineName} has been completed.`
    });
    setShowTimeLogDialog(false);
    setActualTime('');
    setNotes('');
  };

  const renderProductionControlView = () => (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-slate-900">Mold Change Requests</h2>
          <p className="text-slate-600">Manage mold change requests for production</p>
        </div>
      </div>

      <Tabs defaultValue="pending" className="w-full">
        <TabsList className="grid w-full grid-cols-3">
          <TabsTrigger value="pending">Pending Approval</TabsTrigger>
          <TabsTrigger value="approved">Approved</TabsTrigger>
          <TabsTrigger value="rejected">Rejected</TabsTrigger>
        </TabsList>

        <TabsContent value="pending" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <AlertTriangle className="h-5 w-5 text-orange-600" />
                Pending MCRs ({filteredMCRs.filter(mcr => mcr.status === 'pending').length})
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Machine</TableHead>
                    <TableHead>Product</TableHead>
                    <TableHead>From Mold</TableHead>
                    <TableHead>To Mold</TableHead>
                    <TableHead>Scheduled</TableHead>
                    <TableHead>Est. Time</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredMCRs.filter(mcr => mcr.status === 'pending').map((mcr) => (
                    <TableRow key={mcr.id}>
                      <TableCell className="font-medium">{mcr.machineName}</TableCell>
                      <TableCell>{mcr.productName}</TableCell>
                      <TableCell>{mcr.fromMoldName}</TableCell>
                      <TableCell>{mcr.toMoldName}</TableCell>
                      <TableCell>
                        <div className="text-sm">
                          <div>{format(mcr.scheduledDate, 'MMM dd, yyyy')}</div>
                          <div className="text-slate-500">{mcr.scheduledTime}</div>
                        </div>
                      </TableCell>
                      <TableCell>{mcr.estimatedTime} min</TableCell>
                      <TableCell>
                        <div className="flex gap-2">
                          <Button
                            size="sm"
                            variant="outline"
                            onClick={() => {
                              setSelectedMCR(mcr);
                              setShowDetailDialog(true);
                            }}
                          >
                            <Eye className="h-4 w-4 mr-1" />
                            View
                          </Button>
                          <Button
                            size="sm"
                            className="bg-green-600 hover:bg-green-700 text-white"
                            onClick={() => {
                              setSelectedMCR(mcr);
                              setShowApprovalDialog(true);
                            }}
                          >
                            <ThumbsUp className="h-4 w-4 mr-1" />
                            Approve/Reject
                          </Button>
                        </div>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="approved" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <CheckCircle className="h-5 w-5 text-green-600" />
                Approved MCRs ({filteredMCRs.filter(mcr => mcr.status === 'approved').length})
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Machine</TableHead>
                    <TableHead>Product</TableHead>
                    <TableHead>To Mold</TableHead>
                    <TableHead>Approved By</TableHead>
                    <TableHead>Approved At</TableHead>
                    <TableHead>Status</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredMCRs.filter(mcr => mcr.status === 'approved').map((mcr) => (
                    <TableRow key={mcr.id}>
                      <TableCell className="font-medium">{mcr.machineName}</TableCell>
                      <TableCell>{mcr.productName}</TableCell>
                      <TableCell>{mcr.toMoldName}</TableCell>
                      <TableCell>{mcr.approvedBy}</TableCell>
                      <TableCell>
                        {mcr.approvedAt ? format(mcr.approvedAt, 'MMM dd, HH:mm') : '-'}
                      </TableCell>
                      <TableCell>
                        <Badge className={getStatusConfig(mcr.status).color}>
                          {getStatusConfig(mcr.status).icon} {getStatusConfig(mcr.status).text}
                        </Badge>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="rejected" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <XCircle className="h-5 w-5 text-red-600" />
                Rejected MCRs ({filteredMCRs.filter(mcr => mcr.status === 'rejected').length})
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Machine</TableHead>
                    <TableHead>Product</TableHead>
                    <TableHead>To Mold</TableHead>
                    <TableHead>Rejected By</TableHead>
                    <TableHead>Reason</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredMCRs.filter(mcr => mcr.status === 'rejected').map((mcr) => (
                    <TableRow key={mcr.id}>
                      <TableCell className="font-medium">{mcr.machineName}</TableCell>
                      <TableCell>{mcr.productName}</TableCell>
                      <TableCell>{mcr.toMoldName}</TableCell>
                      <TableCell>{mcr.rejectedBy}</TableCell>
                      <TableCell className="max-w-xs truncate">{mcr.rejectionReason}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );

  const renderMoldTechnicianView = () => (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold text-slate-900">Mold Change Tasks</h2>
          <p className="text-slate-600">Execute approved mold change requests</p>
        </div>
      </div>

      <Tabs defaultValue="approved" className="w-full">
        <TabsList className="grid w-full grid-cols-3">
          <TabsTrigger value="approved">Ready to Start</TabsTrigger>
          <TabsTrigger value="in-progress">In Progress</TabsTrigger>
          <TabsTrigger value="completed">Completed</TabsTrigger>
        </TabsList>

        <TabsContent value="approved" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Play className="h-5 w-5 text-green-600" />
                Ready to Start ({filteredMCRs.filter(mcr => mcr.status === 'approved').length})
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Machine</TableHead>
                    <TableHead>Product</TableHead>
                    <TableHead>From Mold</TableHead>
                    <TableHead>To Mold</TableHead>
                    <TableHead>Est. Time</TableHead>
                    <TableHead>Scheduled</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredMCRs.filter(mcr => mcr.status === 'approved').map((mcr) => (
                    <TableRow key={mcr.id}>
                      <TableCell className="font-medium">{mcr.machineName}</TableCell>
                      <TableCell>{mcr.productName}</TableCell>
                      <TableCell>{mcr.fromMoldName}</TableCell>
                      <TableCell>{mcr.toMoldName}</TableCell>
                      <TableCell>{mcr.estimatedTime} min</TableCell>
                      <TableCell>
                        <div className="text-sm">
                          <div>{format(mcr.scheduledDate, 'MMM dd, yyyy')}</div>
                          <div className="text-slate-500">{mcr.scheduledTime}</div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <Button
                          size="sm"
                          className="bg-blue-600 hover:bg-blue-700 text-white"
                          onClick={() => handleAccept(mcr)}
                        >
                          <Play className="h-4 w-4 mr-1" />
                          Start
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="in-progress" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Wrench className="h-5 w-5 text-orange-600" />
                In Progress ({filteredMCRs.filter(mcr => mcr.status === 'in-progress').length})
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Machine</TableHead>
                    <TableHead>To Mold</TableHead>
                    <TableHead>Started At</TableHead>
                    <TableHead>Est. Time</TableHead>
                    <TableHead>Actions</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredMCRs.filter(mcr => mcr.status === 'in-progress').map((mcr) => (
                    <TableRow key={mcr.id}>
                      <TableCell className="font-medium">{mcr.machineName}</TableCell>
                      <TableCell>{mcr.toMoldName}</TableCell>
                      <TableCell>
                        {mcr.startedAt ? format(mcr.startedAt, 'MMM dd, HH:mm') : '-'}
                      </TableCell>
                      <TableCell>{mcr.estimatedTime} min</TableCell>
                      <TableCell>
                        <Button
                          size="sm"
                          className="bg-green-600 hover:bg-green-700 text-white"
                          onClick={() => {
                            setSelectedMCR(mcr);
                            setShowTimeLogDialog(true);
                          }}
                        >
                          <CheckCircle className="h-4 w-4 mr-1" />
                          Complete
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="completed" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <CheckCircle className="h-5 w-5 text-emerald-600" />
                Completed ({filteredMCRs.filter(mcr => mcr.status === 'completed').length})
              </CardTitle>
            </CardHeader>
            <CardContent>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Machine</TableHead>
                    <TableHead>To Mold</TableHead>
                    <TableHead>Completed At</TableHead>
                    <TableHead>Est. Time</TableHead>
                    <TableHead>Actual Time</TableHead>
                    <TableHead>Variance</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {filteredMCRs.filter(mcr => mcr.status === 'completed').map((mcr) => (
                    <TableRow key={mcr.id}>
                      <TableCell className="font-medium">{mcr.machineName}</TableCell>
                      <TableCell>{mcr.toMoldName}</TableCell>
                      <TableCell>
                        {mcr.completedAt ? format(mcr.completedAt, 'MMM dd, HH:mm') : '-'}
                      </TableCell>
                      <TableCell>{mcr.estimatedTime} min</TableCell>
                      <TableCell>{mcr.actualTime || '-'} min</TableCell>
                      <TableCell>
                        {mcr.actualTime ? (
                          <span className={mcr.actualTime > mcr.estimatedTime ? 'text-red-600' : 'text-green-600'}>
                            {mcr.actualTime > mcr.estimatedTime ? '+' : ''}{mcr.actualTime - mcr.estimatedTime} min
                          </span>
                        ) : '-'}
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  );

  return (
    <div className="space-y-6">
      {userRole === 'production-control' ? renderProductionControlView() : renderMoldTechnicianView()}

      {/* MCR Detail Dialog */}
      <Dialog open={showDetailDialog} onOpenChange={setShowDetailDialog}>
        <DialogContent className="max-w-2xl">
          <DialogHeader>
            <DialogTitle>Mold Change Request Details</DialogTitle>
          </DialogHeader>
          {selectedMCR && (
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label className="font-medium">Machine</Label>
                  <div className="text-slate-700">{selectedMCR.machineName}</div>
                </div>
                <div>
                  <Label className="font-medium">Product</Label>
                  <div className="text-slate-700">{selectedMCR.productName}</div>
                </div>
                <div>
                  <Label className="font-medium">From Mold</Label>
                  <div className="text-slate-700">{selectedMCR.fromMoldName}</div>
                </div>
                <div>
                  <Label className="font-medium">To Mold</Label>
                  <div className="text-slate-700">{selectedMCR.toMoldName}</div>
                </div>
                <div>
                  <Label className="font-medium">Estimated Time</Label>
                  <div className="text-slate-700">{selectedMCR.estimatedTime} minutes</div>
                </div>
                <div>
                  <Label className="font-medium">Status</Label>
                  <Badge className={getStatusConfig(selectedMCR.status).color}>
                    {getStatusConfig(selectedMCR.status).icon} {getStatusConfig(selectedMCR.status).text}
                  </Badge>
                </div>
              </div>
            </div>
          )}
        </DialogContent>
      </Dialog>

      {/* Approval Dialog */}
      <Dialog open={showApprovalDialog} onOpenChange={setShowApprovalDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Approve/Reject MCR</DialogTitle>
            <DialogDescription>
              Review and approve or reject this mold change request.
            </DialogDescription>
          </DialogHeader>
          {selectedMCR && (
            <div className="space-y-4">
              <div className="p-4 bg-slate-50 rounded-lg">
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <div><strong>Machine:</strong> {selectedMCR.machineName}</div>
                  <div><strong>Product:</strong> {selectedMCR.productName}</div>
                  <div><strong>From:</strong> {selectedMCR.fromMoldName}</div>
                  <div><strong>To:</strong> {selectedMCR.toMoldName}</div>
                </div>
              </div>
              <div>
                <Label htmlFor="rejection-reason">Rejection Reason (if rejecting)</Label>
                <Textarea
                  id="rejection-reason"
                  value={rejectionReason}
                  onChange={(e) => setRejectionReason(e.target.value)}
                  placeholder="Enter reason for rejection..."
                />
              </div>
            </div>
          )}
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowApprovalDialog(false)}>
              Cancel
            </Button>
            <Button 
              variant="destructive"
              onClick={() => selectedMCR && handleReject(selectedMCR)}
            >
              <XCircle className="h-4 w-4 mr-2" />
              Reject
            </Button>
            <Button 
              className="bg-green-600 hover:bg-green-700"
              onClick={() => selectedMCR && handleApprove(selectedMCR)}
            >
              <CheckCircle className="h-4 w-4 mr-2" />
              Approve
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      {/* Time Log Dialog */}
      <Dialog open={showTimeLogDialog} onOpenChange={setShowTimeLogDialog}>
        <DialogContent>
          <DialogHeader>
            <DialogTitle>Complete Mold Change</DialogTitle>
            <DialogDescription>
              Log the actual time taken and any notes.
            </DialogDescription>
          </DialogHeader>
          {selectedMCR && (
            <div className="space-y-4">
              <div className="p-4 bg-slate-50 rounded-lg">
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <div><strong>Machine:</strong> {selectedMCR.machineName}</div>
                  <div><strong>To Mold:</strong> {selectedMCR.toMoldName}</div>
                  <div><strong>Estimated:</strong> {selectedMCR.estimatedTime} min</div>
                </div>
              </div>
              <div>
                <Label htmlFor="actual-time">Actual Time (minutes)*</Label>
                <Input
                  id="actual-time"
                  type="number"
                  value={actualTime}
                  onChange={(e) => setActualTime(e.target.value)}
                  placeholder="Enter actual time in minutes"
                />
              </div>
              <div>
                <Label htmlFor="completion-notes">Notes</Label>
                <Textarea
                  id="completion-notes"
                  value={notes}
                  onChange={(e) => setNotes(e.target.value)}
                  placeholder="Any additional notes or issues encountered..."
                />
              </div>
            </div>
          )}
          <DialogFooter>
            <Button variant="outline" onClick={() => setShowTimeLogDialog(false)}>
              Cancel
            </Button>
            <Button 
              className="bg-green-600 hover:bg-green-700"
              onClick={() => selectedMCR && handleComplete(selectedMCR)}
            >
              <CheckCircle className="h-4 w-4 mr-2" />
              Complete
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}