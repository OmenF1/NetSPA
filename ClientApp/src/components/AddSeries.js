import React, { Component} from 'react';
import authService from './api-authorization/AuthorizeService'


export class AddSeries extends Component {
    constructor(props) {
        super(props);
        this.inputValue = "";
        this.onSubmit = this.onSubmit.bind(this);
    }
    
    onChange = event => 
    {
        this.inputValue = event.target.value;
    }

    onSubmit(e)
    {
        e.preventDefault();
        this.GetSeries(this.inputValue);
    }

    async GetSeries(series)
    {
        const token = await authService.getAccessToken();
        const response = await fetch('api/series/SearchSeries/' + series, {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        console.log(data);
    }

    render() {
        return (
            <>
                <div className='input-group'>
                    <div className='form-outline'>
                        <input type='search' id='search' className='form-control' placeholder='Search' onChange={this.onChange} />
                    </div>
                    <button type='button' className='btn btn-primary' onClick={this.onSubmit}>
                        Search
                    </button>
                </div>
            </>
        );
    }
}