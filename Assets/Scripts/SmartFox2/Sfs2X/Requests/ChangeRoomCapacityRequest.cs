using System.Collections.Generic;
using Sfs2X.Entities;
using Sfs2X.Exceptions;

namespace Sfs2X.Requests
{
    public class ChangeRoomCapacityRequest : BaseRequest
    {
        public ChangeRoomCapacityRequest()
            : base(RequestType.ChangeRoomCapacity)
        {
        }

        public static readonly string KEY_ROOM = "r";
        public static readonly string KEY_USER_SIZE = "u";
        public static readonly string KEY_SPEC_SIZE = "s";

        private Room room;
        private int newMaxUsers;
        private int newMaxSpect;

        public override void Validate(SmartFox sfs)
        {
            List<string> list = new List<string>();
            if (room == null)
            {
                list.Add("Provided room is null");
            }
            if (list.Count > 0)
            {
                throw new SFSValidationError("ChangeRoomCapacity request error", list);
            }
        }

        public override void Execute(SmartFox sfs)
        {
            sfso.PutInt(KEY_ROOM, room.Id);
            sfso.PutInt(KEY_USER_SIZE, newMaxUsers);
            sfso.PutInt(KEY_SPEC_SIZE, newMaxSpect);
        }
    }
}
