import { BehaviorSubject, Observable } from "rxjs";

export class ComponentService {
    private result: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(false);
    public result$: Observable<boolean> = this.result.asObservable();
  
    updateResult(value: boolean) {
      this.result.next(value);
    }
  }