import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Group, CreateGroupRequest, JoinGroupRequest } from '../models/group.model';

@Injectable({
  providedIn: 'root'
})
export class GroupService {
  private readonly apiUrl = 'http://localhost:5125/api';

  constructor(private http: HttpClient) {}

  // busca todos os grupos do usuário logado
  getMyGroups(): Observable<Group[]> {
    return this.http.get<Group[]>(`${this.apiUrl}/groups`);
  }

  // cria um novo grupo
  createGroup(request: CreateGroupRequest): Observable<Group> {
    return this.http.post<Group>(`${this.apiUrl}/groups`, request);
  }

  // entra num grupo pelo código de convite
  joinGroup(request: JoinGroupRequest): Observable<Group> {
    return this.http.post<Group>(`${this.apiUrl}/groups/join`, request);
  }
}