import { Directive, Input, ViewContainerRef, TemplateRef } from '@angular/core';
import { AuthService } from '../core/services/auth.service';

@Directive({
    standalone: false,
    selector: '[permission]'
})
export class PermissionDirective {
    constructor(
        private authService: AuthService,
        private viewContainer: ViewContainerRef,
        private templateRef: TemplateRef<any>
    ) {}

    @Input()
    set permission(p: string) {
        this.viewContainer.clear();
        if (this.authService.hasPermission(p)) {
            this.viewContainer.createEmbeddedView(this.templateRef);
        }
    }
}
