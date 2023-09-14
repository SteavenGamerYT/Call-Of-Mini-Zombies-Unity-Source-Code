using System.Collections.Generic;
using Sfs2X.Exceptions;
using Sfs2X.Requests;
using Sfs2X.Util;
using Sfs2X.Entities.Data;
using Sfs2X.Requests.Match;

namespace Sfs2X.Requests
{
    public class FindUsersRequest : BaseRequest
    {
        public FindUsersRequest()
            : base(RequestType.FindUsers)
        {
        }

        public static readonly string KEY_EXPRESSION = "e";
        public static readonly string KEY_GROUP = "g";
        public static readonly string KEY_ROOM = "r";
        public static readonly string KEY_LIMIT = "l";
        public static readonly string KEY_FILTERED_USERS = "fu";

        private new SFSObject sfso;
        private MatchExpression matchExpr;
        private object target;
        private int limit;

        public void Validate()
        {
            List<string> list = new List<string>();
            if (matchExpr == null)
            {
                list.Add("Missing Match Expression");
            }
            if (list.Count > 0)
            {
                throw new SFSValidationError("FindUsers request error", list);
            }
        }

        public SFSObject ToSFSObject()
        {
            sfso = new SFSObject();
            sfso.PutSFSArray(KEY_EXPRESSION, matchExpr.ToSFSArray());
            if (target != null)
            {
                if (target is Room)
                {
                    sfso.PutInt(KEY_ROOM, (target as Room).Id);
                }
                else if (target is string)
                {
                    sfso.PutUtfString(KEY_GROUP, target as string);
                }
                else
                {
                    throw new SFSValidationError("Unsupported target type for FindUsersRequest: " + target);
                }
            }
            if (limit > 0)
            {
                sfso.PutShort(KEY_LIMIT, (short)limit);
            }

            return sfso;
        }
    }
}