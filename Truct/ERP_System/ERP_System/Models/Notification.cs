using System;

namespace ERP_System.Models
{
    public class Notification : BaseModel
    {
        private int _notificationID;
        public int NotificationID
        {
            get => _notificationID;
            set => SetProperty(ref _notificationID, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        private string _notificationType;
        public string NotificationType
        {
            get => _notificationType;
            set => SetProperty(ref _notificationType, value);
        }

        private string _referenceType;
        public string ReferenceType
        {
            get => _referenceType;
            set => SetProperty(ref _referenceType, value);
        }

        private int _referenceID;
        public int ReferenceID
        {
            get => _referenceID;
            set => SetProperty(ref _referenceID, value);
        }

        private bool _isRead;
        public bool IsRead
        {
            get => _isRead;
            set => SetProperty(ref _isRead, value);
        }

        private DateTime _createdDate;
        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }
    }
}