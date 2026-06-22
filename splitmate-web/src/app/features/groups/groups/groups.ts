import { Component, OnInit, PLATFORM_ID, Inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule, isPlatformBrowser } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatToolbarModule } from '@angular/material/toolbar';
import { GroupService } from '../../../core/services/group.service';
import { AuthService } from '../../../core/services/auth.service';
import { Group } from '../../../core/models/group.model';

@Component({
  selector: 'app-groups',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatToolbarModule
  ],
  templateUrl: './groups.html',
  styleUrl: './groups.scss'
})
export class GroupsComponent implements OnInit {
  groups: Group[] = [];
  isLoading = true;
  errorMessage = '';
  showCreateForm = false;
  showJoinForm = false;

  createForm: FormGroup;
  joinForm: FormGroup;

  constructor(
    private groupService: GroupService,
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder,
    @Inject(PLATFORM_ID) private platformId: Object,
    private cdr: ChangeDetectorRef
  ) {
    this.createForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]]
    });

    this.joinForm = this.fb.group({
      inviteCode: ['', [Validators.required, Validators.minLength(8)]]
    });
  }

  ngOnInit(): void {
    // só carrega os grupos se estiver no navegador — não no servidor SSR
    if (isPlatformBrowser(this.platformId)) {
      this.loadGroups();
    }
  }

    loadGroups(): void {
    this.isLoading = true;
    console.log('Carregando grupos...');
    this.groupService.getMyGroups().subscribe({
      next: (groups) => {
        console.log('Grupos recebidos:', groups);
        this.groups = groups;
        this.isLoading = false;
        this.cdr.detectChanges();
      },
      error: (err: any) => {
        console.log('Erro:', err);
        this.errorMessage = 'Erro ao carregar grupos.';
        this.isLoading = false;
        this.cdr.detectChanges();
      }
    });
  }

  createGroup(): void {
    if (this.createForm.invalid) return;

    this.groupService.createGroup(this.createForm.value).subscribe({
      next: (group) => {
        this.groups.push(group);
        this.createForm.reset();
        this.showCreateForm = false;
      },
      error: () => {
        this.errorMessage = 'Erro ao criar grupo.';
      }
    });
  }

  joinGroup(): void {
    if (this.joinForm.invalid) return;

    this.groupService.joinGroup(this.joinForm.value).subscribe({
      next: (group) => {
        this.groups.push(group);
        this.joinForm.reset();
        this.showJoinForm = false;
      },
      error: (err: any) => {
        this.errorMessage = err.error?.message || 'Código de convite inválido.';
      }
    });
  }

  openGroup(groupId: string): void {
    this.router.navigate(['/groups', groupId]);
  }

  logout(): void {
    this.authService.logout();
  }

  get currentUserName(): string {
    return this.authService.currentUser?.name || '';
  }
}