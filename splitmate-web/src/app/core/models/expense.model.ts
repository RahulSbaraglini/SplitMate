export interface ExpenseSplit {
  userId: string;
  amount: number;
}

export interface Expense {
  id: string;
  description: string;
  amount: number;
  paidByUserId: string;
  paidByName: string;
  date: string;
  splits: ExpenseSplit[];
}

export interface AddExpenseRequest {
  description: string;
  amount: number;
  paidByUserId: string;
  splits: ExpenseSplit[];
}

export interface Debt {
  fromUserId: string;
  fromUserName: string;
  toUserId: string;
  toUserName: string;
  amount: number;
}