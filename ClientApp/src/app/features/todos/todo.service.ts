import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Todo {
  id: number;
  title: string;
  isCompleted: boolean;
  completedAt: string | null;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class TodoService {
  private readonly apiUrl: string;

  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.apiUrl = baseUrl + 'api/todos';
  }

  getAll(): Observable<Todo[]> {
    return this.http.get<Todo[]>(this.apiUrl);
  }

  create(title: string): Observable<Todo> {
    return this.http.post<Todo>(this.apiUrl, { title });
  }

  update(id: number, changes: { title?: string; isCompleted?: boolean }): Observable<Todo> {
    return this.http.put<Todo>(`${this.apiUrl}/${id}`, changes);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
