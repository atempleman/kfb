import { Component, OnInit, TemplateRef } from '@angular/core';
import { League } from '../_models/league';
import { LeagueService } from '../_services/league.service';
import { AlertifyService } from '../_services/alertify.service';
import { TeamService } from '../_services/team.service';
import { AuthService } from '../_services/auth.service';
import { Team } from '../_models/team';
import { InboxMessage } from '../_models/inboxMessage';
import { ContactService } from '../_services/contact.service';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import {formatDate} from '@angular/common';
import { GetRosterQuickView } from '../_models/getRosterQuickView';

@Component({
  selector: 'app-inbox',
  templateUrl: './inbox.component.html',
  styleUrls: ['./inbox.component.css']
})
export class InboxComponent implements OnInit {
  league: League;
  team: Team;
  messages: InboxMessage[] = [];
  public modalRef: BsModalRef;
  messageState = 0;
  viewedMessage: InboxMessage;
  teams: Team[] = [];
  selectedTeam: number;
  replySubject = '';

  constructor(private leagueService: LeagueService, private alertify: AlertifyService, private teamService: TeamService,
              private authService: AuthService, private contactService: ContactService, private modalService: BsModalService) { }

  ngOnInit() {
    this.teamService.getTeamForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.team = result;
      // Need to persist the team to cookie
      localStorage.setItem('teamId', this.team.teamId.toString());
    }, error => {
      this.alertify.error('Error getting your Team');
    }, () => {
      this.setupLeague();
    });
  }

  setupLeague() {
    this.leagueService.getLeagueForUserId(this.authService.decodedToken.nameid).subscribe(result => {
      this.league = result;
    }, error => {
      this.alertify.error('Error getting League Details');
    }, () => {
      this.setupPage();
    });
  }

  setupPage() {
    this.getMessages();
    
    this.teamService.getAllTeams(this.league.id).subscribe(result => {
      // find the index of the users team
      const index = result.findIndex(x => x.teamId === this.team.teamId);
      result.splice(index, 1);
      this.teams = result;
    }, error => {
      this.alertify.error('Error getting teams');
    });
  }

  getMessages() {
    const summary: GetRosterQuickView = {
      teamId: this.team.teamId,
      leagueId: this.league.id
    };

    this.contactService.getInboxMessages(summary).subscribe(result => {
      this.messages = result;
    }, error => {
      this.alertify.error('Error getting your messages');
    });
  }

  deleteMessage(message: number) {
    this.contactService.deleteInboxMessage(message).subscribe(result => {
    }, error => {
      this.alertify.error('Error deleting message');
    }, () => {
      // Need to update the messages
      this.getMessages();
    });
  }

  deleteMessageFromModal() {
    this.contactService.deleteInboxMessage(this.viewedMessage.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error deleting message');
    }, () => {
      this.getMessages();
      this.modalRef.hide();
    });
  }

  public openModal(template: TemplateRef<any>, message: InboxMessage) {
    this.messageState = 1;
    this.viewedMessage = message;
    this.contactService.markMessageRead(message.id).subscribe(result => {
    }, error => {
      this.alertify.error('Error marking message read');
    });
    this.modalRef = this.modalService.show(template);
  }

  public openModalNew(template: TemplateRef<any>) {
    this.modalRef = this.modalService.show(template);
  }

  replyAction() {
    this.messageState = 2;
    this.replySubject = 'RE: ' + this.viewedMessage.subject;
  }

  updateMessageForm() {
    const bodyValue = (document.getElementById('body') as HTMLInputElement).value;
    const dt = formatDate(new Date(), 'dd/MM/yyyy', 'en');

    const receivingTeam = this.teams.find(x => x.teamId === +this.selectedTeam);

    const message: InboxMessage = {
      id: 0,
      senderId: this.team.teamId,
      senderName: '',
      senderTeam: this.team.mascot,
      receiverId: this.viewedMessage.senderId,
      receiverName: '',
      receiverTeam: this.viewedMessage.senderTeam,
      subject: this.replySubject,
      body: bodyValue,
      messageDate: dt,
      isNew: 1,
      leagueId: this.league.id
    };
    this.contactService.sendInboxMessage(message).subscribe(result => {

    }, error => {
      this.alertify.error('Error sending message');
    }, () => {
      this.modalRef.hide();
    });
  }

  newMessageForm() {
    let subjectValue = '';
    if (this.messageState === 2) {
      subjectValue = this.replySubject;
    } else {
      subjectValue = (document.getElementById('subject') as HTMLInputElement).value;
    }

    const bodyValue = (document.getElementById('body') as HTMLInputElement).value;
    const dt = formatDate(new Date(), 'dd/MM/yyyy', 'en');
    const receivingTeam = this.teams.find(x => x.teamId === +this.selectedTeam);

    let sender = this.team.mascot;
    let receiver = receivingTeam.mascot;
    if (this.messageState === 2) {
      receiver = this.team.mascot;
      sender = receivingTeam.mascot;
    }

    const message: InboxMessage = {
      id: 0,
      senderId: this.team.teamId,
      senderName: '',
      senderTeam: sender,
      receiverId: +this.selectedTeam,
      receiverName: '',
      receiverTeam: receiver,
      subject: subjectValue,
      body: bodyValue,
      messageDate: dt,
      isNew: 1,
      leagueId: this.league.id
    };
    this.contactService.sendInboxMessage(message).subscribe(result => {

    }, error => {
      this.alertify.error('Error sending message');
    }, () => {
      this.modalRef.hide();
    });
  }

  cancelReply() {
    this.messageState = 1;
  }
}
