import { Component, OnInit, PLATFORM_ID, Inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatSelectModule } from '@angular/material/select';
import { ExpenseService } from '../../../core/services/expense.service';
import { GroupService } from '../../../core/services/group.service';
import { AuthService } from '../../../core/services/auth.service';
import { Expense, Debt } from '../../../core/models/expense.model';
import { Group } from '../../../core/models/group.model';

@Component({
  selector: 'app-group-detail',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatToolbarModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatSelectModule
  ],
  templateUrl: './group-detail.html',
  styleUrl: './group-detail.scss'
})
export class GroupDetailComponent implements OnInit {
  group: Group | null = null;
  expenses: Expense[] = [];
  debts: Debt[] = [];
  isLoading = true;
  showAddExpense = false;
  errorMessage = '';

  expenseForm: FormGroup;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private expenseService: ExpenseService,
    private groupService: GroupService,
    private authService: AuthService,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.expenseForm = this.fb.group({
      description: ['', Validators.required],
      amount: ['', [Validators.required, Validators.min(0.01)]],
      paidByUserId: ['', Validators.required],
      splits: this.fb.array([])
    });
  }

  ngOnInit(): void {
    if (isPlatformBrowser(this.platformId)) {
      const groupId = this.route.snapshot.paramMap.get('id')!;
      this.loadGroupData(groupId);
    }
  }

  loadGroupData(groupId: string): void {
    this.isLoading = true;
    this.groupService.getMyGroups().subscribe({
      next: (groups) => {
        this.group = groups.find(g => g.id === groupId) || null;
        if (this.group) {
          this.loadExpenses(groupId);
          this.loadDebts(groupId);
        }
        this.cdr.detectChanges();
      }
    });
  }

  loadExpenses(groupId: string): void {
    this.expenseService.getGroupExpenses(groupId).subscribe({
      next: (expenses: Expense[]) => {
        this.expenses = expenses;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  loadDebts(groupId: string): void {
    this.expenseService.getGroupDebts(groupId).subscribe({
      next: (debts: Debt[]) => {
        this.debts = debts;
        this.cdr.detectChanges();
      }
    });
  }

  get splits(): FormArray {
    return this.expenseForm.get('splits') as FormArray;
  }

  onAmountChange(): void {
    const amount = parseFloat(this.expenseForm.get('amount')?.value || 0);
    const memberCount = this.group?.members.length || 1;
    const splitAmount = Math.round((amount / memberCount) * 100) / 100;
    this.splits.controls.forEach(control => {
      control.get('amount')?.setValue(splitAmount);
    });
  }

  showExpenseForm(): void {
    this.showAddExpense = true;
    this.splits.clear();
    this.group?.members.forEach(member => {
      this.splits.push(this.fb.group({
        userId: [member.userId],
        amount: [0, [Validators.required, Validators.min(0)]]
      }));
    });
  }

  addExpense(): void {
    if (this.expenseForm.invalid) return;

    const groupId = this.route.snapshot.paramMap.get('id')!;
    const formValue = this.expenseForm.value;

    this.expenseService.addExpense(groupId, {
      description: formValue.description,
      amount: parseFloat(formValue.amount),
      paidByUserId: formValue.paidByUserId,
      splits: formValue.splits
    }).subscribe({
      next: (expense: Expense) => {
        this.expenses.push(expense);
        this.expenseForm.reset();
        this.splits.clear();
        this.showAddExpense = false;
        this.loadDebts(groupId);
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        this.errorMessage = err.error?.message || 'Erro ao adicionar gasto.';
        this.cdr.detectChanges();
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/groups']);
  }

  get currentUserId(): string {
    return this.authService.currentUser?.userId || '';
  }

  getMemberName(userId: string): string {
    return this.group?.members.find(m => m.userId === userId)?.name || 'Desconhecido';
  }
}