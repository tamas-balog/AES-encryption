import { Component, OnInit, ViewChild } from '@angular/core';
import { EncryptionService } from '../encryption.service';
import { Round } from '../models/Round';

@Component({
  selector: 'app-encryption',
  templateUrl: './encryption.component.html',
  styleUrls: ['./encryption.component.css']
})
export class EncryptionComponent implements OnInit {

  @ViewChild('inputForm', { static: false }) form: any;
  text: string;
  encryptionKey: string;
  loaded: boolean = false;
  keysToggled: boolean = false;
  keys: string[];
  rounds: Round[] = [];
  invRounds: Round[] = [];
  currentRoundEncryption: number = 1;
  currentRoundDecryption: number = 1;
  encryptionGroup: number;
  decryptionGroup: number;
  currentEncryptionGroup: number = 0;
  currentDecryptionGroup: number = 0;

  constructor(private encryptionService: EncryptionService) {
  }

  ngOnInit() {
  }

  Submit() {
    this.encryptionService.postText(this.text, this.encryptionKey).subscribe(res => {
      this.keys = res.Item1;
      this.rounds = res.Item2;
      this.invRounds = res.Item3;
      this.encryptionGroup = this.rounds.length / 11;
      this.decryptionGroup = this.rounds.length / 11;
      this.loaded = true;
    })
  }

  Togglekeys() {
    this.keysToggled = !this.keysToggled;
  }

  StepEncryption(direction: number) {
    if (direction == 1 && this.currentRoundEncryption < 10) {
      this.currentRoundEncryption++;
    }
    else if (direction == -1 && this.currentRoundEncryption > 0) {
      this.currentRoundEncryption--;
    }
  }
  StepDecryption(direction: number) {
    if (direction == 1 && this.currentRoundDecryption < 10) {
      this.currentRoundDecryption++;
    }
    else if (direction == -1 && this.currentRoundDecryption > 0) {
      this.currentRoundDecryption--;
    }
  }

  StepEncryptionGroup(direction: number) {
    if (direction == 1 && this.currentEncryptionGroup < this.encryptionGroup - 1) {
      this.currentEncryptionGroup++;
    }
    else if (direction == -1 && this.currentEncryptionGroup > 0) {
      this.currentEncryptionGroup--;
    }
  }

  StepDecryptionGroup(direction: number) {
    if (direction == 1 && this.currentDecryptionGroup < this.decryptionGroup - 1) {
      this.currentDecryptionGroup++;
    }
    else if (direction == -1 && this.currentDecryptionGroup > 0) {
      this.currentDecryptionGroup--;
    }
  }
}
