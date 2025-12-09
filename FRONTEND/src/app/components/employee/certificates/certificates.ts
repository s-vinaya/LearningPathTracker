import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { EmployeeService } from '../../../services/employee.service';
import { EmployeeSidebarComponent } from '../sidebar/employee-sidebar';

@Component({
  selector: 'app-certificates',
  standalone: true,
  imports: [CommonModule, EmployeeSidebarComponent],
  templateUrl: './certificates.html',
  styleUrls: ['./certificates.css']
})
export class CertificatesComponent implements OnInit {
  certificates: any[] = [];
  loading = true;
  certificatePreviews: Map<number, SafeResourceUrl> = new Map();
  certificateUrls: Map<number, string> = new Map();

  constructor(
    private employeeService: EmployeeService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit() {
    this.loadCertificates();
  }

  loadCertificates() {
    this.employeeService.getCertificates().subscribe({
      next: (data) => {
        this.certificates = data;
        this.loading = false;
        this.loadPreviews();
      },
      error: (err) => {
        console.error('Error loading certificates:', err);
        this.loading = false;
      }
    });
  }

  loadPreviews() {
    this.certificates.forEach(cert => {
      this.employeeService.previewCertificate(cert.id).subscribe({
        next: (blob) => {
          const url = window.URL.createObjectURL(blob) + '#toolbar=0&navpanes=0&scrollbar=0';
          const safeUrl = this.sanitizer.bypassSecurityTrustResourceUrl(url);
          this.certificatePreviews.set(cert.id, safeUrl);
          this.certificateUrls.set(cert.id, url);
        },
        error: (err) => console.error('Error loading preview:', err)
      });
    });
  }

  getCertificatePreview(certId: number): SafeResourceUrl | undefined {
    return this.certificatePreviews.get(certId);
  }

  downloadCertificate(certificateId: number, name: string) {
    this.employeeService.downloadCertificate(certificateId).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `${name}_certificate.pdf`;
        a.click();
        window.URL.revokeObjectURL(url);
      },
      error: (err) => console.error('Error downloading certificate:', err)
    });
  }

  openCertificate(certificateId: number) {
    const url = this.certificateUrls.get(certificateId);
    if (url) {
      window.open(url, '_blank');
    }
  }
}
