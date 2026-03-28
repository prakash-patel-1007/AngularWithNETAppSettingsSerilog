import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { FormsModule } from '@angular/forms';
import { of, throwError } from 'rxjs';
import { TodoListComponent } from './todo-list.component';
import { TodoService, Todo } from './todo.service';
import { ComponentService } from '../../services/component.service';

describe('TodoListComponent', () => {
  let component: TodoListComponent;
  let fixture: ComponentFixture<TodoListComponent>;
  let todoServiceSpy: jasmine.SpyObj<TodoService>;

  const mockTodos: Todo[] = [
    { id: 1, title: 'Active', isCompleted: false, completedAt: null, createdAt: '2026-03-28T00:00:00Z' },
    { id: 2, title: 'Done', isCompleted: true, completedAt: '2026-03-28T12:00:00Z', createdAt: '2026-03-27T00:00:00Z' }
  ];

  beforeEach(waitForAsync(() => {
    todoServiceSpy = jasmine.createSpyObj('TodoService', ['getAll', 'create', 'update', 'delete']);
    todoServiceSpy.getAll.and.returnValue(of(mockTodos));

    TestBed.configureTestingModule({
      declarations: [TodoListComponent],
      imports: [FormsModule],
      providers: [
        { provide: TodoService, useValue: todoServiceSpy },
        ComponentService
      ]
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TodoListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load todos on init', () => {
    expect(component.todos.length).toBe(2);
    expect(todoServiceSpy.getAll).toHaveBeenCalled();
  });

  it('should separate active and completed todos', () => {
    expect(component.activeTodos.length).toBe(1);
    expect(component.completedTodos.length).toBe(1);
  });

  it('should calculate progress', () => {
    component.todos = [...mockTodos];
    expect(component.completedCount).toBe(1);
    expect(component.progressPercent).toBe(50);
  });

  it('should add a todo', () => {
    const newTodo: Todo = { id: 3, title: 'New', isCompleted: false, completedAt: null, createdAt: '2026-03-28T00:00:00Z' };
    todoServiceSpy.create.and.returnValue(of(newTodo));

    component.newTitle = 'New';
    component.addTodo();

    expect(todoServiceSpy.create).toHaveBeenCalledWith('New');
    expect(component.todos[0].title).toBe('New');
    expect(component.newTitle).toBe('');
  });

  it('should not add todo with empty title', () => {
    component.newTitle = '  ';
    component.addTodo();

    expect(todoServiceSpy.create).not.toHaveBeenCalled();
  });

  it('should toggle todo completion', () => {
    const activeTodo = component.todos.find(t => !t.isCompleted)!;
    const updated: Todo = { ...activeTodo, isCompleted: true, completedAt: '2026-03-28T13:00:00Z' };
    todoServiceSpy.update.and.returnValue(of(updated));

    component.toggleComplete(activeTodo);

    expect(todoServiceSpy.update).toHaveBeenCalledWith(activeTodo.id, { isCompleted: true });
  });

  it('should delete a todo', () => {
    todoServiceSpy.delete.and.returnValue(of(void 0));

    component.deleteTodo(1);

    expect(todoServiceSpy.delete).toHaveBeenCalledWith(1);
    expect(component.todos.find(t => t.id === 1)).toBeUndefined();
  });

  it('should show error on load failure', () => {
    todoServiceSpy.getAll.and.returnValue(throwError(() => new Error('fail')));

    component.loadTodos();

    expect(component.errorMessage).toBe('Failed to load todos.');
    expect(component.isLoading).toBeFalse();
  });

  it('should return 0 progress when no todos', () => {
    component.todos = [];
    expect(component.progressPercent).toBe(0);
  });
});
