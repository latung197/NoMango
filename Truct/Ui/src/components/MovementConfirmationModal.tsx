import React, { useState } from 'react';
import { ShieldCheck, Eye, EyeOff, Loader2 } from 'lucide-react';
import { Dialog, DialogContent, DialogHeader, DialogTitle } from './ui/dialog';
import { Button } from './ui/button';
import { Input } from './ui/input';
import { Label } from './ui/label';
import { Textarea } from './ui/textarea';
import { toast } from 'sonner';

interface MovementConfirmationModalProps {
  isOpen: boolean;
  onClose: () => void;
  onConfirm: (reason?: string) => void;
  title: string;
  titleMM: string;
  currentUser?: string;
}

export function MovementConfirmationModal({
  isOpen,
  onClose,
  onConfirm,
  title,
  titleMM,
  currentUser = "staff001@example.com"
}: MovementConfirmationModalProps) {
  const [password, setPassword] = useState('');
  const [reason, setReason] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  const handleConfirm = async () => {
    if (!password.trim()) {
      setError('Password is required | စကားဝှက်လိုအပ်ပါသည်');
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      // Simulate password verification delay
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // Mock password validation (in real app, this would be server-side)
      if (password === 'demo123') {
        onConfirm(reason);
        const username = currentUser.split('@')[0];
        toast.success(`Movement confirmed by ${username}. | ${username} မှ အတည်ပြုပြီးပါပြီ။`);
        handleClose();
      } else {
        setError('Incorrect password | စကားဝှက်မမှန်ကန်ပါ');
      }
    } catch (err) {
      setError('Authentication failed | အထောက်အထားစစ်ဆေးမှုမအောင်မြင်ပါ');
    } finally {
      setIsLoading(false);
    }
  };

  const handleClose = () => {
    setPassword('');
    setReason('');
    setError('');
    setShowPassword(false);
    setIsLoading(false);
    onClose();
  };

  return (
    <Dialog open={isOpen} onOpenChange={handleClose}>
      <DialogContent className="max-w-[560px] rounded-2xl shadow-lg">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-3 text-xl">
            <ShieldCheck className="h-6 w-6 text-green-600" />
            <div>
              <div>{title}</div>
              <div className="text-sm font-normal text-slate-600 mt-1">{titleMM}</div>
            </div>
          </DialogTitle>
        </DialogHeader>
        
        <div className="space-y-6 p-6">
          <div className="text-center">
            <p className="text-sm text-slate-600">
              Enter your password to proceed.
            </p>
            <p className="text-sm text-slate-500 mt-1">
              အတည်ပြုရန် စကားဝှက်ထည့်ပါ။
            </p>
          </div>

          <div className="space-y-4">
            {/* Username (pre-filled, read-only) */}
            <div>
              <Label htmlFor="username">Username</Label>
              <Input
                id="username"
                value={currentUser}
                readOnly
                className="bg-slate-50 cursor-not-allowed"
              />
            </div>

            {/* Password with eye toggle */}
            <div>
              <Label htmlFor="password">Password *</Label>
              <div className="relative">
                <Input
                  id="password"
                  type={showPassword ? 'text' : 'password'}
                  value={password}
                  onChange={(e) => {
                    setPassword(e.target.value);
                    setError(''); // Clear error on typing
                  }}
                  placeholder="Enter your password"
                  className={error ? 'border-red-500' : ''}
                />
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  className="absolute right-2 top-1/2 -translate-y-1/2 h-auto p-1 hover:bg-transparent"
                  onClick={() => setShowPassword(!showPassword)}
                >
                  {showPassword ? (
                    <EyeOff className="h-4 w-4 text-slate-400" />
                  ) : (
                    <Eye className="h-4 w-4 text-slate-400" />
                  )}
                </Button>
              </div>
              {error && (
                <p className="text-sm text-red-600 mt-1">{error}</p>
              )}
            </div>

            {/* Reason / Notes (optional) */}
            <div>
              <Label htmlFor="reason">Reason / Notes (optional)</Label>
              <Textarea
                id="reason"
                value={reason}
                onChange={(e) => setReason(e.target.value)}
                placeholder="Additional notes or reason for this movement..."
                rows={3}
                className="resize-none"
              />
            </div>
          </div>

          {/* Footer buttons */}
          <div className="flex justify-end gap-3 pt-4">
            <Button
              variant="ghost"
              onClick={handleClose}
              disabled={isLoading}
            >
              Cancel | ပယ်ဖျက်မည်
            </Button>
            <Button
              className="bg-green-600 hover:bg-green-700"
              onClick={handleConfirm}
              disabled={!password.trim() || isLoading}
            >
              {isLoading ? (
                <>
                  <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                  Confirming...
                </>
              ) : (
                'Confirm | အတည်ပြုမည်'
              )}
            </Button>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  );
}