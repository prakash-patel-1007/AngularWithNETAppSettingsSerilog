import { Component, OnInit } from '@angular/core';
import { TodoService, Todo } from './todo.service';
import { ComponentService } from '../../services/component.service';

@Component({
  standalone: false,
  selector: 'app-todo-list',
  templateUrl: './todo-list.component.html',
  styleUrls: ['./todo-list.component.css']
})
export class TodoListComponent implements OnInit {
  todos: Todo[] = [];
  newTitle = '';
  isLoading = false;
  errorMessage = '';

  constructor(
    private todoService: TodoService,
    private componentService: ComponentService,
  ) {
    this.componentService.updateResult(true);
  }

  get activeTodos(): Todo[] {
    return this.todos.filter(t => !t.isCompleted);
  }

  get completedTodos(): Todo[] {
    return this.todos.filter(t => t.isCompleted);
  }

  get completedCount(): number {
    return this.completedTodos.length;
  }

  get progressPercent(): number {
    return this.todos.length === 0 ? 0 : (this.completedCount / this.todos.length) * 100;
  }

  trackById(_index: number, todo: Todo): number {
    return todo.id;
  }

  ngOnInit(): void {
    this.loadTodos();
  }

  loadTodos(): void {
    this.isLoading = true;
    this.errorMessage = '';
    this.todoService.getAll().subscribe({
      next: todos => { this.todos = todos; this.isLoading = false; },
      error: () => { this.errorMessage = 'Failed to load todos.'; this.isLoading = false; }
    });
  }

  addTodo(): void {
    const title = this.newTitle.trim();
    if (!title) return;
    this.todoService.create(title).subscribe({
      next: todo => { this.todos.unshift(todo); this.newTitle = ''; },
      error: () => { this.errorMessage = 'Failed to create todo.'; }
    });
  }

  toggleComplete(todo: Todo): void {
    this.todoService.update(todo.id, { isCompleted: !todo.isCompleted }).subscribe({
      next: updated => {
        const idx = this.todos.findIndex(t => t.id === todo.id);
        if (idx >= 0) this.todos[idx] = updated;
      },
      error: () => { this.errorMessage = 'Failed to update todo.'; }
    });
  }

  deleteTodo(id: number): void {
    this.todoService.delete(id).subscribe({
      next: () => { this.todos = this.todos.filter(t => t.id !== id); },
      error: () => { this.errorMessage = 'Failed to delete todo.'; }
    });
  }
}
