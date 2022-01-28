import { Observable, empty } from 'rxjs';
import { HttpEventType, HttpErrorResponse, HttpResponse } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Injectable } from '@angular/core';

const readFile = (blob: Blob): Observable<string> => Observable.create(obs => {
  if (!(blob instanceof Blob)) {
    obs.error(new Error('Unknown error'));
    return;
  }
  const reader = new FileReader();
  reader.onerror = err => obs.error(err);
  reader.onabort = err => obs.error(err);
  reader.onload = () => obs.next(reader.result);
  reader.onloadend = () => obs.complete();
  return reader.readAsText(blob);
});

@Injectable()
export class ServiceBaseConfiguration {
  constructor() { }
}

@Injectable()
export class ServiceBase {

  constructor(
    private serviceBaseConfiguration: ServiceBaseConfiguration
  ) {
  }

  public transformOptions(options) {
    options.reportProgress = true;
    options.observe = 'events';
    return Promise.resolve(options);
  }

  public transformResult(url: string, response: any, processor: (response: any) => any): Observable<any> {
    let type: HttpEventType = response.type;
    switch (type) {
      case HttpEventType.Sent:
        // The request was sent out over the wire.
        break;
      case HttpEventType.UploadProgress:
      // An upload progress event was received.
      case HttpEventType.DownloadProgress:
        // A download progress event was received.
        break;
      case HttpEventType.ResponseHeader:
        // The response status code and headers were received.
        break;
      case HttpEventType.Response:
        //  // The full response including the body was received.
        if (response instanceof HttpErrorResponse) {
          return readFile(response.error).pipe(map(value => {
            throw <IErrorInfo>JSON.parse(value);
          }));
        } else if (event instanceof HttpResponse) {
          console.log('File is completely uploaded!');
        }
        return processor(response);
      case HttpEventType.User:
      // A custom event from an interceptor or a backend.
      default:
        return processor(response);
    }
    return empty();
  }
}

export interface IErrorInfo {
  isError: boolean;
  message: string;
  details: string;
}
