import { TestBed } from '@angular/core/testing';
import { provideHttpClientTesting, HttpTestingController } from '@angular/common/http/testing';
import { provideHttpClient } from '@angular/common/http';
import { TodoService, Todo } from './todo.service';

describe('TodoService', () => {
  let service: TodoService;
  let httpMock: HttpTestingController;

  const mockTodo: Todo = {
    id: 1,
    title: 'Test',
    isCompleted: false,
    completedAt: null,
    createdAt: '2026-03-28T00:00:00Z'
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        { provide: 'BASE_URL', useValue: '/' },
        TodoService
      ]
    });

    service = TestBed.inject(TodoService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpMock.verify());

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getAll should GET /api/todos', () => {
    service.getAll().subscribe(todos => {
      expect(todos.length).toBe(1);
      expect(todos[0].title).toBe('Test');
    });

    const req = httpMock.expectOne('/api/todos');
    expect(req.request.method).toBe('GET');
    req.flush([mockTodo]);
  });

  it('create should POST /api/todos', () => {
    service.create('New Todo').subscribe(todo => {
      expect(todo.title).toBe('New Todo');
    });

    const req = httpMock.expectOne('/api/todos');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ title: 'New Todo' });
    req.flush({ ...mockTodo, title: 'New Todo' });
  });

  it('update should PUT /api/todos/:id', () => {
    service.update(1, { isCompleted: true }).subscribe(todo => {
      expect(todo.isCompleted).toBeTrue();
    });

    const req = httpMock.expectOne('/api/todos/1');
    expect(req.request.method).toBe('PUT');
    expect(req.request.body).toEqual({ isCompleted: true });
    req.flush({ ...mockTodo, isCompleted: true, completedAt: '2026-03-28T12:00:00Z' });
  });

  it('delete should DELETE /api/todos/:id', () => {
    service.delete(1).subscribe();

    const req = httpMock.expectOne('/api/todos/1');
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});
