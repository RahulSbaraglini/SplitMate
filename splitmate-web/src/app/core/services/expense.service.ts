import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Expense, AddExpenseRequest, Debt } from '../models/expense.model';

@Injectable({
  providedIn: 'root'
})
export class ExpenseService {
  private readonly apiUrl = 'http://localhost:5125/api';

  constructor(private http: HttpClient) {}

  getGroupExpenses(groupId: string): Observable<Expense[]> {
    return this.http.get<Expense[]>(`${this.apiUrl}/groups/${groupId}/expenses`);
  }

  addExpense(groupId: string, request: AddExpenseRequest): Observable<Expense> {
    return this.http.post<Expense>(`${this.apiUrl}/groups/${groupId}/expenses`, request);
  }

  getGroupDebts(groupId: string): Observable<Debt[]> {
    return this.http.get<Debt[]>(`${this.apiUrl}/groups/${groupId}/expenses/debts`);
  }
}