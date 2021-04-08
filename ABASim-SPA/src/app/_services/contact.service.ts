import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ContactForm } from '../_models/contactForm';
import { environment } from 'src/environments/environment';
import { GlobalChat } from '../_models/globalChat';
import { InboxMessage } from '../_models/inboxMessage';
import { Observable } from 'rxjs';
import { CountOfMessages } from '../_models/countOfMessages';
import { GetRosterQuickView } from '../_models/getRosterQuickView';

@Injectable({
  providedIn: 'root'
})
export class ContactService {
  baseUrl = environment.apiUrl + '/contact/';

  constructor(private http: HttpClient) { }

  saveContact(contactForm: ContactForm) {
    return this.http.post(this.baseUrl + 'savecontact', contactForm);
  }

  sendChat(chat: GlobalChat) {
    return this.http.post(this.baseUrl + 'savechatrecord', chat);
  }

  getChatRecords(leagueId: number): Observable<GlobalChat[]> {
    return this.http.get<GlobalChat[]>(this.baseUrl + 'getchatrecords/' + leagueId);
  }

  getInboxMessages(tl: GetRosterQuickView): Observable<InboxMessage[]> {
    const params = new HttpParams()
      .set('teamId', tl.teamId.toString())
      .set('leagueId', tl.leagueId.toString());

    return this.http.get<InboxMessage[]>(this.baseUrl + 'getinboxmessages', { params });
  }

  sendInboxMessage(message: InboxMessage) {
    return this.http.post(this.baseUrl + 'sendinboxmessage', message);
  }

  deleteInboxMessage(message: number) {
    return this.http.get<boolean>(this.baseUrl + 'deletemessage/' + message);
  }

  getCountOfMessages(tl: GetRosterQuickView) {
    const params = new HttpParams()
      .set('teamId', tl.teamId.toString())
      .set('leagueId', tl.leagueId.toString());

    return this.http.get<CountOfMessages>(this.baseUrl + 'getMessageCount', { params });
  }

  markMessageRead(messageId: number) {
    return this.http.get<boolean>(this.baseUrl + 'markasread/' + messageId);
  }
}
