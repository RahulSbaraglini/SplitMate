export interface GroupMember {
  userId: string;
  name: string;
}

export interface Group {
  id: string;
  name: string;
  inviteCode: string;
  memberCount: number;
  members: GroupMember[];
}

export interface CreateGroupRequest {
  name: string;
}

export interface JoinGroupRequest {
  inviteCode: string;
}