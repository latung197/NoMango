# Factory Dashboard Setup Instructions

## Required Dependencies

To use the drag-and-drop functionality in the Factory Dashboard, you need to install the following packages:

```bash
npm install react-dnd react-dnd-html5-backend
```

## Features Implemented

### 1. **Live Machine Monitoring**
- Real-time machine status display with color-coded tiles
- Machine efficiency tracking with progress rings
- Operator information with photos and ratings
- Live alerts and warnings system
- Temperature and pressure monitoring

### 2. **Factory KPI Summary Cards**
- Running Machines (Green) - Shows count and percentage
- Idle Machines (Yellow) - Machines waiting for jobs
- Blocked/Down Machines (Red) - Machines with issues
- Total Production (Blue) - Units produced today with target
- Defects (Orange) - Defect count with percentage and top reason

### 3. **Machine Tiles with Comprehensive Info**
- **Status Ring Charts** - Visual progress indicators
- **Operator Photos & Names** - With bilingual support
- **Product Images & Job Details** - Current production info
- **Real-time Alerts** - Material shortages, maintenance needs
- **Performance Metrics** - Efficiency, cycle time, OEE scores
- **Schedule Information** - Start time, estimated finish, work orders

### 4. **Drag-and-Drop Production Planning**
- **Gantt-style Timeline** - 24-hour view with hour-by-hour slots
- **Draggable Job Cards** - Move jobs between machines and time slots
- **Conflict Detection** - Auto-alerts for scheduling conflicts
- **Material Availability** - Checks before job assignment
- **Color-coded Job States** - Running (Green), Scheduled (Blue), Maintenance (Orange)

### 5. **Intelligent Alerts System**
- **Critical Alerts** - Machine downtime > 30 minutes
- **Warning Alerts** - Material shortages, quality issues
- **Info Alerts** - Target achievements, schedule updates
- **Bilingual Support** - English and Myanmar languages

### 6. **Quick Actions Sidebar**
- **Add New Job** - Modal with product, quantity, machine selection
- **Reschedule Jobs** - Direct access to planning timeline
- **Material Requests** - Quick inventory requests
- **Maintenance Alerts** - Report machine issues

### 7. **Analytics & Reports**
- **Machine Efficiency Charts** - Real-time performance tracking
- **Downtime Analysis** - Breakdown by reason codes
- **Material Stock Status** - Live inventory levels
- **Operator Leaderboard** - Top performers ranking

### 8. **Design Features**
- **Factory-Friendly UI** - Large fonts, bold colors, visible from 3 meters
- **Idiot-Proof Design** - Clear visual indicators, intuitive layout
- **Status Colors**: 🟢 Green (Running), 🟡 Yellow (Idle), 🔴 Red (Blocked), 🔵 Blue (Scheduled), 🟠 Orange (Maintenance)
- **Responsive Layout** - Works on desktop and large factory displays
- **Auto-refresh** - Updates every 30 seconds
- **Touch-Friendly** - Large buttons for kiosk operation

## Navigation

The Factory Dashboard is accessible through:
- Main Navigation: "Smart Factory Dashboard" (စမတ်စက်ရုံ ထိန်းချုပ်ရေး)
- URL: Set currentPage to 'factory-dashboard'

## Tab Structure

1. **📊 Live Monitoring** - Main dashboard with KPIs and machine tiles
2. **📅 Production Planning** - Drag-and-drop scheduling timeline
3. **📈 Analytics** - Performance charts and downtime analysis
4. **📋 Reports** - Detailed production reports (placeholder)

## Mock Data

The dashboard includes comprehensive mock data for:
- 6 injection molding machines with different statuses
- 5 operators with photos and ratings
- 5 product types with images
- Real-time alerts and notifications
- Planned jobs for scheduling
- Material stock levels
- Performance metrics

This creates a fully functional, modern factory dashboard that combines real-time monitoring with intelligent production planning capabilities.