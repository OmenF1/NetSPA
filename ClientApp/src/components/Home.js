import React, { Component } from 'react';
import {ListWatching} from './ListWatching';

export class Home extends Component {

  render() {
    return (
      <div>
        <h1>NetSPA</h1>
        <h3>Tracking your Netflix progress.</h3>
        <ListWatching />     
      </div>
    );
  }
}
