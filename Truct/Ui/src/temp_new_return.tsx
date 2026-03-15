    return (
      <div className="space-y-6">
        {/* Header */}
        <div className="bg-gradient-to-r from-green-50 to-blue-50 rounded-lg p-6 border border-green-200">
          <h1 className="text-2xl font-semibold text-slate-900">
            Finished Goods Intake (Scan to Accept) | ကုန်ချောလက်ခံ (စကင်ဖြင့် လက်ခံ)
          </h1>
          <p className="text-slate-600 mt-2">
            FG Warehouse Manager scans labels created at Production Control after QC approval
          </p>
          <p className="text-sm text-slate-500 mt-1">
            ⚠️ စကင်သာလုပ်ရန် - လိပ်စာများကို Production Control တွင် QC အတည်ပြုပြီးနောက် ဖန်တီးထားပြီးဖြစ်သည်
          </p>
        </div>

        <div className="grid grid-cols-1 xl:grid-cols-3 gap-6">
          {/* Left Column - Scan Panel */}
          <div className="xl:col-span-1">
            {/* Scanner Input */}
            <Card className="border-2 border-green-200 shadow-md mb-6">
              <CardHeader className="bg-green-50">
                <CardTitle className="flex items-center gap-2 text-green-800">
                  <Scan className="h-6 w-6" />
                  <span>Scan & Accept Panel | စကင်လုပ်ပြီး လက်ခံရန်</span>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <Label className="text-lg font-medium">
                    Scan Label to Accept | လက်ခံရန် လိပ်စာကို စကင်လုပ်ပါ
                  </Label>
                  <div className="flex gap-2 mt-2">
                    <Input
                      value={scanInput}
                      onChange={(e) => setScanInput(e.target.value)}
                      placeholder="Focus here & scan QR/RFID | စကင်ရန် ဤနေရာတွင် အာရုံစိုက်ပါ"
                      className="bg-yellow-50 border-2 border-yellow-300 text-lg h-16 font-mono"
                      onKeyPress={(e) => e.key === 'Enter' && handleScan()}
                      autoFocus
                    />
                    <Button 
                      onClick={handleScan}
                      className="bg-green-600 hover:bg-green-700 h-16 px-6"
                      size="lg"
                    >
                      <Scan className="h-5 w-5 mr-2" />
                      Scan
                    </Button>
                  </div>
                  <p className="text-sm text-slate-500 mt-2">
                    Input auto-focused for scanner | စကင်နာအတွက် အလိုအလျောက် အာရုံစိုက်ထားသည်
                  </p>
                </div>

                {/* Running Total */}
                {runningTotal > 0 && (
                  <div className="bg-blue-50 rounded-lg p-4 border border-blue-200">
                    <div className="flex items-center justify-between">
                      <span className="font-medium text-blue-800">Running Total | စုစုပေါင်း</span>
                      <div className="text-2xl font-bold text-blue-600">
                        {runningTotal.toLocaleString()} pcs
                      </div>
                    </div>
                    <p className="text-sm text-blue-600 mt-1">Multiple scans building pallet | များစွာစကင်လုပ်၍ ပိုင်တင်ဖွဲ့စည်းခြင်း</p>
                  </div>
                )}

                {/* Scanned Product Card */}
                {scannedProduct && (
                  <div className="bg-gradient-to-r from-green-50 to-green-100 rounded-lg p-4 border-2 border-green-300">
                    <h3 className="font-semibold text-green-800 mb-3">Scanned Product | စကင်လုပ်ထားသော ထုတ်ကုန်</h3>
                    <div className="space-y-3">
                      <div>
                        <div className="font-medium text-lg">{scannedProduct.product}</div>
                        <div className="text-sm text-slate-600">{scannedProduct.productMM}</div>
                      </div>
                      <div className="grid grid-cols-2 gap-3 text-sm">
                        <div>
                          <span className="text-slate-500">Job ID:</span>
                          <div className="font-medium">{scannedProduct.jobId}</div>
                        </div>
                        <div>
                          <span className="text-slate-500">Released Qty:</span>
                          <div className="font-medium text-blue-600">{scannedProduct.qtyReleased.toLocaleString()}</div>
                        </div>
                        <div>
                          <span className="text-slate-500">Pending Qty:</span>
                          <div className="font-medium text-orange-600">{scannedProduct.pendingQty.toLocaleString()}</div>
                        </div>
                      </div>

                      {/* Accept Form */}
                      <div className="space-y-3 pt-3 border-t border-green-300">
                        <div>
                          <Label>Accept Qty | လက်ခံအရေအတွက်</Label>
                          <Input
                            type="number"
                            value={acceptedQty}
                            onChange={(e) => setAcceptedQty(e.target.value)}
                            placeholder="Default = remaining"
                            className="bg-white h-12 text-lg"
                          />
                          <p className="text-xs text-slate-500 mt-1">Default = remaining | မူလအတိုင်း = ကျန်ရှိ</p>
                        </div>

                        <div className="grid grid-cols-2 gap-2">
                          <div>
                            <Label>Rack | စင်</Label>
                            <Select value={selectedRack} onValueChange={setSelectedRack}>
                              <SelectTrigger className="bg-white">
                                <SelectValue placeholder="Optional" />
                              </SelectTrigger>
                              <SelectContent>
                                {mockRacks.map((rack) => (
                                  <SelectItem key={rack} value={rack}>{rack}</SelectItem>
                                ))}
                              </SelectContent>
                            </Select>
                          </div>
                          <div>
                            <Label>Bin | ပုံး</Label>
                            <Select value={selectedBin} onValueChange={setSelectedBin}>
                              <SelectTrigger className="bg-white">
                                <SelectValue placeholder="Optional" />
                              </SelectTrigger>
                              <SelectContent>
                                {mockBins.map((bin) => (
                                  <SelectItem key={bin} value={bin}>{bin}</SelectItem>
                                ))}
                              </SelectContent>
                            </Select>
                          </div>
                        </div>

                        <div>
                          <Label>Note | မှတ်ချက်</Label>
                          <Input 
                            value={rejectReason}
                            onChange={(e) => setRejectReason(e.target.value)}
                            placeholder="Optional note"
                            className="bg-white"
                          />
                        </div>

                        <div className="flex gap-2">
                          <Button 
                            onClick={handleAccept}
                            className="bg-green-600 hover:bg-green-700 flex-1 h-12"
                          >
                            <CheckCircle className="h-4 w-4 mr-2" />
                            Accept | လက်ခံမည်
                          </Button>
                          <Button 
                            variant="outline"
                            onClick={() => {
                              setScannedProduct(null);
                              setAcceptedQty('');
                              setSelectedRack('');
                              setSelectedBin('');
                              setRejectReason('');
                            }}
                            className="h-12"
                          >
                            Cancel | ပယ်ဖျက်မည်
                          </Button>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
              </CardContent>
            </Card>
          </div>

          {/* Right Column - Queue & History */}
          <div className="xl:col-span-2 space-y-6">
            {/* Top Filters */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <div className="flex items-center justify-between">
                  <CardTitle className="flex items-center gap-2">
                    <Filter className="h-5 w-5 text-slate-600" />
                    <span>Filters | စစ်ထုတ်မှု</span>
                  </CardTitle>
                  <Button variant="outline" size="sm">
                    <RefreshCw className="h-4 w-4 mr-2" />
                    Refresh | ပြန်လည်ဖွင့်ရန်
                  </Button>
                </div>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                  <div>
                    <Label>Release ID / Job ID</Label>
                    <Input placeholder="Search..." className="bg-white" />
                  </div>
                  <div>
                    <Label>Product | ထုတ်ကုန်</Label>
                    <Input placeholder="Search product" className="bg-white" />
                  </div>
                  <div>
                    <Label>QC Status | QC အခြေအနေ</Label>
                    <Select defaultValue="approved">
                      <SelectTrigger className="bg-white">
                        <SelectValue />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">All Statuses</SelectItem>
                        <SelectItem value="approved">Approved | အတည်ပြုပြီး</SelectItem>
                        <SelectItem value="pending">Pending | ဆိုင်းငံ့</SelectItem>
                        <SelectItem value="rejected">Rejected | ငြင်းပယ်</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                  <div>
                    <Label>Date Range | ရက်စွဲအပိုင်းအခြား</Label>
                    <Input type="date" className="bg-white" defaultValue="2025-09-07" />
                  </div>
                </div>
              </CardContent>
            </Card>

            {/* Intake Queue Table */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Package className="h-5 w-5 text-green-600" />
                  <span>Intake Queue Table | လက်ခံစောင့်ရန်စာရင်း</span>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Release ID | ထုတ်ပေးကုဒ်</TableHead>
                        <TableHead>Job ID | အလုပ်ကုဒ်</TableHead>
                        <TableHead>Product (EN/MM)</TableHead>
                        <TableHead>Qty Released</TableHead>
                        <TableHead>QC Status</TableHead>
                        <TableHead>Released By</TableHead>
                        <TableHead>Released Time</TableHead>
                        <TableHead>Pending Qty</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {mockReleasedBatches.map((batch) => (
                        <TableRow key={batch.releaseId} className="hover:bg-slate-50">
                          <TableCell className="font-medium font-mono">{batch.releaseId}</TableCell>
                          <TableCell className="font-medium font-mono">{batch.jobId}</TableCell>
                          <TableCell>
                            <div>
                              <div className="font-medium">{batch.product}</div>
                              <div className="text-sm text-slate-500">{batch.productMM}</div>
                            </div>
                          </TableCell>
                          <TableCell className="font-medium text-blue-600">
                            {batch.qtyReleased.toLocaleString()} pcs
                          </TableCell>
                          <TableCell>
                            <Badge className="bg-green-100 text-green-800">
                              <CheckCircle className="h-3 w-3 mr-1" />
                              Approved | အတည်ပြုပြီး
                            </Badge>
                          </TableCell>
                          <TableCell>
                            <div className="flex items-center gap-1">
                              <User className="h-3 w-3 text-slate-400" />
                              {batch.releasedBy}
                            </div>
                          </TableCell>
                          <TableCell className="text-sm text-slate-600">{batch.releasedTime}</TableCell>
                          <TableCell className="font-medium text-orange-600">
                            {batch.pendingQty.toLocaleString()} pcs
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </CardContent>
            </Card>

            {/* FG-WH Stock History */}
            <Card className="border-0 shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Archive className="h-5 w-5 text-slate-600" />
                  <span>FG-WH Stock History | FG-WH စတော့မှတ်တမ်း</span>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="overflow-x-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Product | ထုတ်ကုန်</TableHead>
                        <TableHead>Accepted Qty</TableHead>
                        <TableHead>Label ID</TableHead>
                        <TableHead>Job ID</TableHead>
                        <TableHead>Accepted By</TableHead>
                        <TableHead>Date & Time</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {mockFGHistory.map((item) => (
                        <TableRow key={item.id}>
                          <TableCell>
                            <div>
                              <div className="font-medium">{item.product}</div>
                              <div className="text-sm text-slate-500">{item.productMM}</div>
                            </div>
                          </TableCell>
                          <TableCell className="font-medium text-green-600">
                            {item.acceptedQty.toLocaleString()} pcs
                          </TableCell>
                          <TableCell className="font-mono text-sm font-medium">
                            {item.labelId}
                          </TableCell>
                          <TableCell className="font-mono font-medium">
                            {item.jobId}
                          </TableCell>
                          <TableCell>
                            <div className="flex items-center gap-1">
                              <User className="h-3 w-3 text-slate-400" />
                              {item.acceptedBy}
                            </div>
                          </TableCell>
                          <TableCell className="text-sm text-slate-600">{item.dateTime}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
                <div className="mt-4 text-center">
                  <p className="text-sm text-slate-500">
                    <span className="text-green-600 font-medium">✓ Accepted to FG-WH | ကုန်ချော ဂိုဒေါင်သို့ လက်ခံပြီး</span>
                    {' • '}
                    <span className="text-orange-600 font-medium">◐ Partial Accept | အပိုင်းလိုက် လက်ခံ</span>
                  </p>
                </div>
              </CardContent>
            </Card>
          </div>
        </div>
      </div>
    );